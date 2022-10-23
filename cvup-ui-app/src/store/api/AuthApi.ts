import { UserRegistrationModel } from "../../models/Auth";
import BaseApi from "./BaseApi";

export default class AuthApi extends BaseApi {
  // eslint-disable-next-line
  constructor() {
    super();
  }

  async registerUser(registrationInfo: UserRegistrationModel) {
    return await this.promiseWrapper(async () => {
      //   const response = await this.http.get<any>("Search");

      //   return response.data;

      const response = await this.http.post("Registration", registrationInfo);
      return response.data;
    });
  }
}
