import {
  IForgotPassword,
  TokensModel,
  IUserLogin,
  IUserRegistration,
  IInterviewer,
} from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class AuthApi extends BaseApi {
  async registerUser(registrationInfo: IUserRegistration) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Auth/Registration", registrationInfo))
        .data;
      return data;
    });

    return response;
  }

  async login(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const data = (await this.http.post<TokensModel>("Auth/Login", loginInfo))
        .data;
      return data;
    });
  }

  async forgotPassword(info: IForgotPassword) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Auth/ForgotPassword", info)).data;
      return data;
    });

    return response;
  }

  async revoke() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.post<TokensModel>("Auth/revoke")).data;
      return data;
    });
  }

  async completeRegistration(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.post<TokensModel>(
          "Auth/CompleteRegistration",
          loginInfo
        )
      ).data;
      return data;
    });
  }

  async passwordReset(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.post<TokensModel>("Auth/PasswordReset", loginInfo)
      ).data;
      return data;
    });
  }

  async addUpdateInterviewer(interviewer: IInterviewer) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("Auth/AddUpdateInterviewer", interviewer)
      ).data;
      return data;
    });

    return response;
  }

  async getInterviewersList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IInterviewer[]>("Auth/GetInterviewersList")
      ).data;
      return data;
    });

    return response;
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete(`Auth/DeleteInterviewer?id=${interviewer.id}`)
      ).data;
      return data;
    });

    return response;
  }
}
