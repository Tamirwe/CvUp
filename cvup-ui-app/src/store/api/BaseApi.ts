import { AxiosInstance } from "axios";
import { ResponseModel } from "../../models/AuthModels";
import { IAppSettings } from "../../models/GeneralModels";
import axiosService from "../../services/AxiosService";

export default abstract class BaseApi {
  http: AxiosInstance;

  constructor(appSettings: IAppSettings) {
    this.http = axiosService(appSettings.apiUrl);
  }

  async apiWrapper<T>(apiCall: () => Promise<T>): Promise<ResponseModel<T>> {
    try {
      const response = await apiCall();
      // return Promise.resolve({ data, isSuccess: true, error: "" });
      return Promise.resolve({
        data: response,
        isSuccess: true,
        errorData: "",
        status: 0,
      });
    } catch (error: any) {
      const { data, status } = error.response;
      return Promise.resolve({
        data: null as any,
        isSuccess: false,
        errorData: data,
        status,
      });
    }
  }

  async apiWrapper2<T>(apiCall: () => Promise<any>): Promise<ResponseModel<T>> {
    try {
      const response = await apiCall();
      // return Promise.resolve({ data, isSuccess: true, error: "" });
      return Promise.resolve({
        data: response ? (response.data as T) : response,
        isSuccess: true,
        errorData: "",
        status: response ? response.status : "",
      });
    } catch (error: any) {
      return Promise.resolve({
        data: null as any,
        isSuccess: false,
        errorData: error,
        status: error.response ? error.response.status : "",
      });
    }
  }
}
