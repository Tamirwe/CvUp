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
    return await this.apiWrapper2(async () => {
      return await this.http.post("Auth/Registration", registrationInfo);
    });
  }

  async login(loginInfo: IUserLogin) {
    return await this.apiWrapper2<TokensModel>(async () => {
      return await this.http.post("Auth/Login", loginInfo);
    });
  }

  async forgotPassword(info: IForgotPassword) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Auth/ForgotPassword", info);
    });
  }

  async revoke() {
    return await this.apiWrapper2<TokensModel>(async () => {
      return await this.http.post("Auth/revoke");
    });
  }

  async completeRegistration(loginInfo: IUserLogin) {
    return await this.apiWrapper2<TokensModel>(async () => {
      return await this.http.post("Auth/CompleteRegistration", loginInfo);
    });
  }

  async passwordReset(loginInfo: IUserLogin) {
    return await this.apiWrapper2<TokensModel>(async () => {
      return;
      await this.http.post("Auth/PasswordReset", loginInfo);
    });
  }

  async addUpdateInterviewer(interviewer: IInterviewer) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Auth/AddUpdateInterviewer", interviewer);
    });
  }

  async getInterviewersList() {
    return await this.apiWrapper2<IInterviewer[]>(async () => {
      return await this.http.get("Auth/GetInterviewersList");
    });
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(
        `Auth/DeleteInterviewer?id=${interviewer.id}`
      );
    });
  }

  async getUsersList() {
    return await this.apiWrapper2<IUser[]>(async () => {
      return await this.http.get("Auth/GetUsers");
    });
  }

  async addUser(userModel: IUser) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Auth/AddCompanyUser", userModel);
    });
  }

  async updateUser(userModel: IUser) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Auth/UpdateCompanyUser", userModel);
    });
  }

  async deleteUser(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("Auth/DeleteCompanyUser?id=" + id);
    });
  }

  async resendRegistrationEmail(userModel: IUser) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Auth/ResendRegistrationEmail", userModel);
    });
  }

  async activateUser(userModel: IUser) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Auth/ActivateCompanyUser", userModel);
    });
  }

  async dactivateUser(userModel: IUser) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Auth/DactivateCompanyUser", userModel);
    });
  }
}
