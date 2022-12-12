export interface IUserRegistration {
  firstName: string;
  lastName: string;
  companyName: string;
  email: string;
  password: string;
}

export interface IPosition {
  id: number;
  name: string;
  descr: string;
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

export interface IDepartment {
  id: number;
  name: string;
}