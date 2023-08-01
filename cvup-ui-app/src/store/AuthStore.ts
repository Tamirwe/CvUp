import { makeAutoObservable, runInAction } from "mobx";
import { LOGIN_TYPE } from "../constants/AuthConsts";
import {
  IForgotPassword,
  IInterviewer,
  IUser,
  IUserData,
  IUserLogin,
  IUserRegistration,
} from "../models/AuthModels";
import { UserRoleEnum } from "../models/GeneralEnums";
import { IAppSettings, IUserClaims } from "../models/GeneralModels";
import AuthApi from "./api/AuthApi";
import { RootStore } from "./RootStore";

export const TOKEN = "jwt";
export const REFRESH_TOKEN = "refreshToken";

export class AuthStore {
  private authApi;
  private userSelected?: IUser;
  private isUserAppDirRtl = true;

  isLoggedIn = false;
  claims: IUserClaims = {};
  userRole: UserRoleEnum = UserRoleEnum.User;
  interviewersList: IInterviewer[] = [];
  usersList: IUser[] = [];
  currentUserData?: IUserData;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.authApi = new AuthApi(appSettings);
    const jwt = localStorage.getItem(TOKEN);

    if (jwt) {
      this.isLoggedIn = true;
      this.claims = JSON.parse(window.atob(jwt.split(".")[1]));
      this.userRole =
        UserRoleEnum[this.claims.role as unknown as keyof typeof UserRoleEnum];
    }
  }

  reset() {
    localStorage.removeItem(TOKEN);
    localStorage.removeItem(REFRESH_TOKEN);
    this.isLoggedIn = false;
    this.claims = {};
    this.interviewersList = [];
  }

  get isRtl() {
    return this.isUserAppDirRtl;
  }

  get selectedUser() {
    return this.userSelected;
  }

  set selectedUser(val: IUser | undefined) {
    this.userSelected = val;
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
        localStorage.setItem(TOKEN, response.data.token);
        localStorage.setItem(REFRESH_TOKEN, response.data.refreshToken);
        this.claims = JSON.parse(
          window.atob(response.data.token.split(".")[1])
        );
        this.userRole =
          UserRoleEnum[
            this.claims.role as unknown as keyof typeof UserRoleEnum
          ];
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
    if (this.interviewersList.length === 0 || loadAgain) {
      const res = await this.authApi.getInterviewersList();
      runInAction(() => {
        this.interviewersList = res.data;
      });
    }
  }

  async deleteInterviewer(interviewer: IInterviewer) {
    return await this.authApi.deleteInterviewer(interviewer);
  }

  async getUsersList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.authApi.getUsersList();
    runInAction(() => {
      this.usersList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async addUser(user: IUser) {
    user.addedByName = this.claims.DisplayName;
    user.addedById = this.claims.UserId;
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.addUser(user);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateUser(user: IUser) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.updateUser(user);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteUser(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.deleteUser(id);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async activateUser(user: IUser) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.activateUser(user);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async dactivateUser(user: IUser) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.dactivateUser(user);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async resendRegistrationEmail(user: IUser) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.authApi.resendRegistrationEmail(user);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async getUser() {
    const response = await this.authApi.getUser();
    this.currentUserData = response.data;
  }
}
