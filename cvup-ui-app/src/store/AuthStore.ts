import { makeAutoObservable } from "mobx";
import { LOGIN_TYPE } from "../constants/AuthConsts";
import {
  ForgotPasswordModel,
  UserLoginModel,
  UserRegistrationModel,
} from "../models/AuthModels";
import { ClaimsModel } from "../models/GeneralModels";
import AuthApi from "./api/AuthApi";
import { RootStore } from "./RootStore";

export class AuthStore {
  private authApi;
  isLoggedIn = false;
  claims: ClaimsModel = {};
  CompanyId = null;
  UserId = null;
  DisplayName = null;
  email = null;
  role = null;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.authApi = new AuthApi();
    const jwt = localStorage.getItem("jwt");

    if (jwt) {
      this.isLoggedIn = true;
      this.claims = JSON.parse(window.atob(jwt.split(".")[1]));
    }
  }

  async registerUser(registrationInfo: UserRegistrationModel) {
    return await this.authApi.registerUser(registrationInfo);
  }

  async login(loginInfo: UserLoginModel, loginType: string) {
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

  async forgotPassword(info: ForgotPasswordModel) {
    return await this.authApi.forgotPassword(info);
  }
}
