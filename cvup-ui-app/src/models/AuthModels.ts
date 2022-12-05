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

export interface PositionFormModel {
  name: string;
  descr: string;
}
