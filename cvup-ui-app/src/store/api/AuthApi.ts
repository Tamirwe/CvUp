import {
  IForgotPassword,
  TokensModel,
  IUserLogin,
  IUserRegistration,
  IInterviewer,
  IUser,
} from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class AuthApi extends BaseApi {
  async registerUser(registrationInfo: IUserRegistration) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("Auth/Registration", registrationInfo)
      ).data;
      return response;
    });

    return responseData;
  }

  async login(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.post<TokensModel>("Auth/Login", loginInfo)
      ).data;
      return response;
    });
  }

  async forgotPassword(info: IForgotPassword) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.post("Auth/ForgotPassword", info)).data;
      return response;
    });

    return responseData;
  }

  async revoke() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.post<TokensModel>("Auth/revoke")).data;
      return response;
    });
  }

  async completeRegistration(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.post<TokensModel>(
          "Auth/CompleteRegistration",
          loginInfo
        )
      ).data;
      return response;
    });
  }

  async passwordReset(loginInfo: IUserLogin) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.post<TokensModel>("Auth/PasswordReset", loginInfo)
      ).data;
      return response;
    });
  }

  async addUpdateInterviewer(interviewer: IInterviewer) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("Auth/AddUpdateInterviewer", interviewer)
      ).data;
      return response;
    });

    return responseData;
  }

  async getInterviewersList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.get<IInterviewer[]>("Auth/GetInterviewersList")
      ).data;
      return response;
    });

    return responseData;
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.delete(`Auth/DeleteInterviewer?id=${interviewer.id}`)
      ).data;
      return response;
    });

    return responseData;
  }

  async getUsersList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.get<IUser[]>("Auth/GetUsers")).data;
      return response;
    });

    return responseData;
  }

  async addUser(userModel: IUser) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.post("Auth/AddCompanyUser", userModel))
        .data;
      return response;
    });

    return responseData;
  }

  async updateUser(userModel: IUser) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.put("Auth/UpdateCompanyUser", userModel)
      ).data;
      return response;
    });

    return responseData;
  }

  async deleteUser(id: number) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.delete("Auth/DeleteCompanyUser?id=" + id)
      ).data;
      return response;
    });

    return responseData;
  }

  async resendRegistrationEmail(userModel: IUser) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.put("Auth/ResendRegistrationEmail", userModel)
      ).data;
      return response;
    });

    return responseData;
  }
}
