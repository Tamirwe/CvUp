import { makeAutoObservable } from "mobx";
import { UserRegistrationModel } from "../models/Auth";
import AuthApi from "./api/AuthApi";
import { RootStore } from "./RootStore";

export class AuthStore {
  private authApi;
  userName = "Tamir";

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.authApi = new AuthApi();
  }

  registerUser(registrationInfo: UserRegistrationModel) {
    this.authApi.registerUser(registrationInfo);
  }
}
