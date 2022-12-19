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

export class AuthStore {
  private authApi;
  isLoggedIn = false;
  claims: ClaimsModel = {};
  interviewersList: IInterviewer[] | undefined;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.authApi = new AuthApi();
    const jwt = localStorage.getItem("jwt");

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
        localStorage.setItem("jwt", response.data.token);
        localStorage.setItem("refreshToken", response.data.refreshToken);
      }

      return response.isSuccess;
    }
    return false;
  }

  async logout() {
    this.claims = {};
    await this.authApi.revoke();
    localStorage.removeItem("jwt");
    localStorage.removeItem("refreshToken");
  }

  async forgotPassword(info: IForgotPassword) {
    return await this.authApi.forgotPassword(info);
  }

  async addUpdateInterviewer(interviewer: IInterviewer) {
    return await this.authApi.addUpdateInterviewer(interviewer);
  }

  async getInterviewers(loadAgain: boolean) {
    if (!this.interviewersList || loadAgain) {
      const res = await this.authApi.getInterviewers();
      runInAction(() => {
        this.interviewersList = res.data;
      });
    }
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    return await this.authApi.deleteInterviewer(interviewer);
  }
}
