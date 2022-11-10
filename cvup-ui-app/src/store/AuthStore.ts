import { makeAutoObservable } from "mobx";
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

  async login(loginInfo: UserLoginModel) {
    const response = await this.authApi.login(loginInfo);

    if (response.isSuccess) {
      this.isLoggedIn = true;
      localStorage.setItem("jwt", response.data.token);
      localStorage.setItem("refreshToken", response.data.refreshToken);
    }

    return response.isSuccess;
  }

  logout() {
    this.claims = {};
    localStorage.removeItem("jwt");
    localStorage.removeItem("refreshToken");
  }

  async forgotPassword(info: ForgotPasswordModel) {
    return await this.authApi.forgotPassword(info);
  }
}
