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

export interface ICvListItem {
  cvId: string;
  encriptedId: string;
  phone?: string;
  email?: string;
  emailSubject: string;
  candidateName?: string;
}

export interface ICv {
  cvId: string;
  encriptedId: string;
  phone?: string;
  email?: string;
  emailSubject: string;
  candidateName?: string;
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
