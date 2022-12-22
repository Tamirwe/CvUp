import { makeAutoObservable, runInAction } from "mobx";
import { LOGIN_TYPE } from "../constants/AuthConsts";
import {
  IForgotPassword,
  IInterviewer,
  IUserLogin,
  IUserRegistration,
} from "../models/AuthModels";
import { ClaimsModel } from "../models/GeneralModels";
import AuthApi from "./api/AuthApi";
import { RootStore } from "./RootStore";

export const TOKEN = "jwt";
export const REFRESH_TOKEN = "refreshToken";

export class AuthStore {
  private authApi;
  isLoggedIn = false;
  claims: ClaimsModel = {};
  interviewersList: IInterviewer[] | undefined;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.authApi = new AuthApi();
    const jwt = localStorage.getItem(TOKEN);

    if (jwt) {
      this.isLoggedIn = true;
      this.claims = JSON.parse(window.atob(jwt.split(".")[1]));
    }
  }

  async registerUser(registrationInfo: IUserRegistration) {
    return await this.authApi.registerUser(registrationInfo);
  }

  async login(loginInfo: IUserLogin, loginType: string) {
    let response;
    localStorage.removeItem(TOKEN);
    localStorage.removeItem(REFRESH_TOKEN);

    switch (loginType) {
      case LOGIN_TYPE.REGULAR_LOGIN:
        response = await this.authApi.login(loginInfo);
        break;
      case LOGIN_TYPE.COMPLETE_REGISTRATION_LOGIN:
        response = await this.authApi.completeRegistration(loginInfo);
        break;
      case LOGIN_TYPE.PASSWORD_RESET_LOGIN:
        response = await this.authApi.passwordReset(loginInfo);
        break;
    }

    if (response) {
      if (response.isSuccess) {
        this.isLoggedIn = true;
        localStorage.setItem(TOKEN, response.data.token);
        localStorage.setItem(REFRESH_TOKEN, response.data.refreshToken);
      }

      return response.isSuccess;
    }
    return false;
  }

  async logout() {
    this.claims = {};
    await this.authApi.revoke();
    localStorage.removeItem(TOKEN);
    localStorage.removeItem(REFRESH_TOKEN);
  }

  async forgotPassword(info: IForgotPassword) {
    return await this.authApi.forgotPassword(info);
  }

  async addUpdateInterviewer(interviewer: IInterviewer) {
    return await this.authApi.addUpdateInterviewer(interviewer);
  }

  async getInterviewersList(loadAgain: boolean) {
    if (!this.interviewersList || loadAgain) {
      const res = await this.authApi.getInterviewersList();
      runInAction(() => {
        this.interviewersList = res.data;
      });
    }
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    return await this.authApi.deleteInterviewer(interviewer);
  }

}
