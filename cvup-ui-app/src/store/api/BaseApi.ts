import { ResponseModel } from "../../models/AuthModels";
import axiosService from "../../services/AxiosService";

export default abstract class BaseApi {
  http = axiosService("https://localhost:7217/api/");

  async apiWrapper<T>(apiCall: () => Promise<T>): Promise<ResponseModel<T>> {
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

// import axiosService from "../../services/AxiosService";

// export default class BaseApi {
//   http = axiosService("https://localhost:7217/api/");

//   async apiWrapper(apiCall: () => Promise<any>) {
//     try {
//       const data = await apiCall();
//       return Promise.resolve({ data, isSuccess: true, error: false });
//     } catch (error: any) {
//       const { errorMessage } = error;
//       return Promise.resolve({
//         isSuccess: false,
//         error: errorMessage,
//         data: null,
//       });
//     }
//   }
// }
