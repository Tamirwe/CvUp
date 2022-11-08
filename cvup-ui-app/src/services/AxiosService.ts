import axios, { AxiosInstance } from "axios";

export default function axiosService(
  baseURL?: string,
  headers?: any
): AxiosInstance {
  const instance = axios.create({
    baseURL: baseURL,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json; charset=utf-8",
      Authorization: "Bearer " + localStorage.getItem("jwt"),
      ...headers,
    },
  });

  // instance.interceptors.response.use(
  //   (res) => res,
  //   (err) => {
  //     if (err.response) {
  //       return Promise.reject(err.response.data);
  //     }

  //     if (err.request) {
  //       return Promise.reject(err.request);
  //     }

  //     return Promise.reject(err.message);
  //   }
  // );

  // instance.interceptors.request.use(
  //   async (response) => {
  //     // const token = await getToken();
  //     // if (token) {
  //     //   config.headers.Authorization = token;
  //     // }

  //     // if (logRequests) {
  //     //   console.log(
  //     //     `%c ${config?.method?.toUpperCase()} - ${getUrl(config)}:`,
  //     //     "color: #0086b3; font-weight: bold",
  //     //     config
  //     //   );
  //     // }

  //     // config.paramsSerializer = (params) => {
  //     //   return qs.stringify(params, {
  //     //     serializeDate: (date: Date) =>
  //     //       moment(date).format("YYYY-MM-DDTHH:mm:ssZ"),
  //     //   });
  //     // };

  //     return response;
  //   },
  //   (error) => Promise.reject(error)
  // );

  return instance;
}
