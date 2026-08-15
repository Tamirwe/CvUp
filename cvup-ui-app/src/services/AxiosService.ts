import axios, { AxiosError, AxiosInstance, AxiosRequestConfig } from "axios";
import { TokensModel } from "../models/AuthModels";

const REQUEST_TIMEOUT_MS = 15000;

type AuthRequestConfig = AxiosRequestConfig & {
  _retry?: boolean;
  _skipAuthRefresh?: boolean;
};

type RefreshResult =
  | { outcome: "renewed"; tokens: TokensModel }
  | { outcome: "refused" } // the server rejected the tokens, they are dead
  | { outcome: "unreachable" }; // we never got an answer, the tokens may still be good

export default function axiosService(
  baseURL?: string,
  headers?: any
): AxiosInstance {
  const instance = axios.create({
    baseURL: baseURL,
    timeout: REQUEST_TIMEOUT_MS,
    headers: {
      ...headers,
    },
  });

  const requestRefresh = async (): Promise<RefreshResult> => {
    const refreshToken = localStorage.getItem("refreshToken") || "";
    const token = localStorage.getItem("jwt") || "";

    if (!refreshToken || !token) {
      return { outcome: "refused" };
    }

    try {
      const res = await instance.post<TokensModel | "">(
        "Auth/Refresh",
        { token, refreshToken },
        { _skipAuthRefresh: true } as AuthRequestConfig
      );

      // Older builds of the API answer 200 with an empty body when they decline.
      if (!res.data || !res.data.token) {
        return { outcome: "refused" };
      }

      return { outcome: "renewed", tokens: res.data };
    } catch (error: any) {
      const status = (error as AxiosError).response?.status;

      // No response at all means the request never reached the server - a sleeping
      // laptop, a dropped wifi, a timeout. Those tokens are probably still valid,
      // so keep them and let the next request try again.
      return status ? { outcome: "refused" } : { outcome: "unreachable" };
    }
  };

  // The refresh token is single use: the server rotates it on every call. Parallel
  // 401s must therefore share one refresh, or every call but the first would be
  // sent with an already-spent token and wipe the session the winner just renewed.
  let pendingRefresh: Promise<RefreshResult> | null = null;

  const refreshAccessToken = () => {
    if (!pendingRefresh) {
      pendingRefresh = requestRefresh().finally(() => {
        pendingRefresh = null;
      });
    }

    return pendingRefresh;
  };

  instance.interceptors.request.use(
    async (config) => {
      const jwt = localStorage.getItem("jwt");

      if (jwt) {
        config.headers = {
          ...config.headers,
          Accept: "application/json",
          "Content-Type": "application/json; charset=utf-8",
          authorization: `Bearer ${jwt}`,
        };
      }

      return config;
    },
    (error) => Promise.reject(error)
  );

  instance.interceptors.response.use(
    (res) => res,
    async (error: AxiosError) => {
      const originalRequest = error.config as AuthRequestConfig | undefined;

      // error.response is undefined for network errors and timeouts, so it has to
      // be checked before its status is read. _skipAuthRefresh keeps a failing
      // refresh from recursing back into the refresh.
      if (
        originalRequest &&
        !originalRequest._retry &&
        !originalRequest._skipAuthRefresh &&
        error.response?.status === 401
      ) {
        originalRequest._retry = true;
        const result = await refreshAccessToken();

        if (result.outcome === "renewed") {
          localStorage.setItem("jwt", result.tokens.token);
          localStorage.setItem("refreshToken", result.tokens.refreshToken);
          return instance(originalRequest);
        }

        if (result.outcome === "refused") {
          localStorage.removeItem("jwt");
          localStorage.removeItem("refreshToken");
        }
      }

      return Promise.reject(error);
    }
  );

  return instance;
}
