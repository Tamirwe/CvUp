import { makeAutoObservable } from "mobx";
import {
  ForgotPasswordModel,
  UserLoginModel,
  UserRegistrationModel,
} from "../models/AuthModels";
import AuthApi from "./api/AuthApi";
import { RootStore } from "./RootStore";

export class AuthStore {
  private authApi;
  userName = "Tamir";
  isLoggedIn = false;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.authApi = new AuthApi();
  }

  async registerUser(registrationInfo: UserRegistrationModel) {
    return await this.authApi.registerUser(registrationInfo);
  }

  async loginUser(loginInfo: UserLoginModel) {
    return await this.authApi.loginUser(loginInfo);
  }

  async forgotPassword(info: ForgotPasswordModel) {
    return await this.authApi.forgotPassword(info);
  }

}
