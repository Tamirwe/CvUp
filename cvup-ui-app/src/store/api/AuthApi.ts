import {
  ForgotPasswordModel,
  UserLoginModel,
  UserRegistrationModel,
} from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class AuthApi extends BaseApi {
  // eslint-disable-next-line
  constructor() {
    super();
  }

  async registerUser(registrationInfo: UserRegistrationModel) {
    const response = await this.apiWrapper(async () => {
      return await this.http.post("Registration", registrationInfo);
    });

    return response;
  }

  async loginUser(loginInfo: UserLoginModel) {
    const response = await this.apiWrapper(async () => {
      return await this.http.post("Login", loginInfo);
    });

    return response;
  }

  async forgotPassword(info: ForgotPasswordModel) {
    const response = await this.apiWrapper(async () => {
      return await this.http.post("ForgotPassword", info);
    });

    return response;
  }
}
