import axiosService from "../../services/AxiosService";

export default class BaseApi {
  http = axiosService("https://localhost:7217/api/");

  async apiWrapper(apiCall: () => Promise<any>) {
    try {
      const data = await apiCall();
      return Promise.resolve({ data, isSuccess: true, error: false });
    } catch (error: any) {
      const { errorMessage } = error;
      return Promise.resolve({
        isSuccess: false,
        error: errorMessage,
        data: null,
      });
    }
  }
}