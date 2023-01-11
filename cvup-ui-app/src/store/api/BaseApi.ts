import { AxiosInstance } from "axios";
import { ResponseModel } from "../../models/AuthModels";
import { IAppSettings } from "../../models/GeneralModels";
import axiosService from "../../services/AxiosService";

export default abstract class BaseApi {
  http: AxiosInstance;
  // http2 = axiosService(appSettings.appServerUrl);
  // http = axiosService("https://localhost:7217/api/");
  // http = axiosService("http://localhost:8010/api/");
  // http = axiosService("https://192.168.1.20:8010/api/");
  // http = axiosService("https://89.237.94.86:443/api/");
  // http = axiosService("http://89.237.94.86:8010/api/");
  // http = axiosService("https://localhost:446/api/");
  // http = axiosService("https://89.237.94.86:446/api/");

  // get http() {
  //   return axiosService(appSettings.appServerUrl);
  // }

  constructor(appSettings: IAppSettings) {
    this.http = axiosService(appSettings.appServerUrl);
  }

  async apiWrapper<T>(apiCall: () => Promise<T>): Promise<ResponseModel<T>> {
    try {
      const data = await apiCall();
      // return Promise.resolve({ data, isSuccess: true, error: "" });
      return Promise.resolve({ data, isSuccess: true, error: "" });
    } catch (error: any) {
      const { errorMessage } = error;
      return Promise.resolve({
        data: null as any,
        isSuccess: false,
        error: errorMessage,
      });
    }
  }
}
