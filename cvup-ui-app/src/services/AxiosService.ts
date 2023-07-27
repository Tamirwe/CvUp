import axios, { AxiosInstance } from "axios";
import { TokensModel } from "../models/AuthModels";

export default function axiosService(
  baseURL?: string,
  headers?: any
): AxiosInstance {
  const instance = axios.create({
    baseURL: baseURL,
    headers: {
      ...headers,
    },
  });

  const refreshAccessToken = async () => {
    const refreshToken = localStorage.getItem("refreshToken") || "";
    const token = localStorage.getItem("jwt") || "";
    const newTokens: TokensModel = { token, refreshToken };

    try {
      const res = (await instance.post<TokensModel>("Auth/Refresh", newTokens))
        .data;
      return res;
    } catch (error: any) {
      return null;
    }
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
    async (error) => {
      const originalRequest = error.config;

      if (error.response.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        const newTokens = await refreshAccessToken();
        originalRequest._retry = true;

        if (newTokens) {
          localStorage.setItem("jwt", newTokens.token);
          localStorage.setItem("refreshToken", newTokens.refreshToken);

          axios.defaults.headers.common["Authorization"] =
            "Bearer " + newTokens.token;
          return instance(originalRequest);
        }
        // else {
        //   localStorage.removeItem("jwt");
        //   localStorage.removeItem("refreshToken");
        //   // document.location.href = "/";
        // }
      }

      return Promise.reject(error);
    }
  );

  return instance;
}
