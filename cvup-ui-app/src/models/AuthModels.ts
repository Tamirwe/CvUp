import { PermissionTypeEnum } from "./GeneralEnums";

export interface IUser {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  teamId: number;
}
export interface IUserRegistration {
  firstName: string;
  lastName: string;
  companyName: string;
  email: string;
  password: string;
}

export interface IUserLogin {
  email: string;
  password: string;
  rememberMe: boolean;
  key?: string;
}

export interface ResponseModel<T> {
  data: T;
  isSuccess: boolean;
  error: string;
}

export interface IForgotPassword {
  email: string;
  companyId: number;
}

export interface TokensModel {
  token: string;
  refreshToken: string;
}

export interface IContact {
  id: number;
  name: string;
  email: string;
  phone: string;
  position: string;
  companyId: number;
}

export interface IHrCompany {
  id: number;
  name: string;
  companyId: number;
  contacts: IContact[];
}

export interface IIdName {
  id: number;
  name: string;
}

export interface IInterviewer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  permissionType: PermissionTypeEnum;
}
