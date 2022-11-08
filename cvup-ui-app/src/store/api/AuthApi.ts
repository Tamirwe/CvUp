import {
  ForgotPasswordModel,
  TokensModel,
  UserLoginModel,
  UserRegistrationModel,
} from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class AuthApi extends BaseApi {
  // eslint-disable-next-line

  async registerUser(registrationInfo: UserRegistrationModel) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Registration", registrationInfo))
        .data;
      return data;
    });

    return response;
  }

  async login(loginInfo: UserLoginModel) {
    return await this.apiWrapper(async () => {
      const data = (await this.http.post<TokensModel>("Login", loginInfo)).data;
      return data;
    });
  }

  async forgotPassword(info: ForgotPasswordModel) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("ForgotPassword", info)).data;
      return data;
    });

    return response;
  }
}
