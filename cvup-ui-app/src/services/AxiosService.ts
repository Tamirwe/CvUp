import axios, { AxiosRequestConfig } from "axios";

function AxiosService(url: string, header: string) {
  const instance = axios.create({
    baseURL: url,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json; charset=utf-8",
    },
  });

  instance.interceptors.response.use(
    (res) => res.data,
    (err) => {
      if (err.response) {
        return Promise.reject(err.response.data);
      }

      if (err.request) {
        return Promise.reject(err.request);
      }

      return Promise.reject(err.message);
    }
  );

  const genericInstance = <T>(config: AxiosRequestConfig) =>
    instance.request<any, T>(config);

  return genericInstance;
}
