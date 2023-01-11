export interface IAppSettings {
  appServerUrl: string;
}
export interface ISelectBox {
  id: number;
  name: string;
  isSelected?: boolean;
}
export interface IUserClaims {
  CompanyId?: string;
  UserId?: string;
  DisplayName?: string;
  email?: string;
  role?: string;
}

export interface ICv {
  cvId: number;
  candId: number;
  keyId: string;
  encriptedId: string;
  phone?: string;
  email?: string;
  emailSubject: string;
  candidateName?: string;
  reviewHtml: string;
  reviewTct: string;
}

export interface IPosition {
  id: number;
  name: string;
  descr: string;
  isActive: boolean;
  departmentId: number;
  hrCompaniesIds: number[];
  interviewersIds: number[];
}

export interface IPositionListItem {
  id: number;
  name: string;
  updated: Date;
  isActive: boolean;
}

export interface ICvReview {
  candidateId: number;
  cvId: number;
  reviewHtml: string;
  reviewText: string;
}
