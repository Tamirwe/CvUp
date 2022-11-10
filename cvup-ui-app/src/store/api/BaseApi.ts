import { ResponseModel, TokensModel } from "../../models/AuthModels";
import axiosService from "../../services/AxiosService";

export default abstract class BaseApi {
  http = axiosService("https://localhost:7217/api/");

  async apiWrapper<T>(apiCall: () => Promise<T>): Promise<ResponseModel<T>> {
    try {
      const data = await apiCall();
      return Promise.resolve({ data, isSuccess: true, error: "" });
    } catch (error: any) {
      // if (error.request.status === 401) {
      //   const refreshToken = localStorage.getItem("refreshToken") || "";
      //   const token = localStorage.getItem("jwt") || "";

      //   const mmm: TokensModel = { token, refreshToken };

      //   const dddd = await this.apiErrorWrapper(async () => {
      //     return (await this.http.post<TokensModel>("Auth/Refresh", mmm)).data;
      //   });

      //   localStorage.setItem("jwt", dddd.data.token);
      //   localStorage.setItem("refreshToken", dddd.data.refreshToken);
      //   console.log(dddd);

      //   const newData = await apiCall();
      //   console.log(newData);
      //   return Promise.resolve({ data: newData, isSuccess: true, error: "" });

      //   //const data = await apiCall();
      // }

      const { errorMessage } = error;
      return Promise.resolve({
        data: null as any,
        isSuccess: false,
        error: errorMessage,
      });
    }
  }

  async apiErrorWrapper<T>(
    apiCall: () => Promise<T>
  ): Promise<ResponseModel<T>> {
    try {
      const data = await apiCall();
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
