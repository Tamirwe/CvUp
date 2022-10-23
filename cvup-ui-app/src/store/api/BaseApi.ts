import axiosService from "../../services/AxiosService";

export default class BaseApi {
  http = axiosService("https://localhost:7217/api/");

  async promiseWrapper(apiCall: () => Promise<BaseApi>) {
    try {
      const data = await apiCall();
      return { data, isSuccess: true };
    } catch (e: any) {
      const { error } = e;

      let errorStr = "";

      if (error) {
        if (error.errors && error.errors.lenght > 0) {
          errorStr = error.errors[0];
        } else {
          errorStr =
            error.title ||
            error.message ||
            (error.StatusCode === 500 ? "500 (Internal Server Error)" : null) ||
            error.Description ||
            "Server error";
        }
      }

      return Promise.resolve({
        isSuccess: false,
        error: errorStr,
        exception: error,
        data: null as any,
      });
    } finally {
    }
  }
}
