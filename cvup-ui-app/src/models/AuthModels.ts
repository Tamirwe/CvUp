export interface UserRegistrationModel {
  firstName: string;
  lastName: string;
  companyName: string;
  email: string;
  password: string;
}
export interface textFieldInterface {
  error?: boolean;
  value: string;
  helperText?: string;
}

export interface UserLoginModel {
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

export interface ForgotPasswordModel {
  email: string;
  companyId?: number;
}

export interface TokensModel {
  token: string;
  refreshToken: string;
}

