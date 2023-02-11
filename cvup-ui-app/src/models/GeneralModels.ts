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

export interface ICand {
  cvId: number;
  review?: string;
  candidateId: number;
  keyId: string;
  encriptedId: string;
  phone?: string;
  email?: string;
  emailSubject: string;
  candidateName?: string;
  reviewTct: string;
  fileType: string;
  cvSent: Date;
  candPosIds: number[];
  cvPosIds: number[];
  stageId: number;
  dateAttached: Date;
  candCvs: IPosCandCvs[];
  hasDuplicates: boolean;
}

export interface IPosCandCvs {
  cvId: number;
  keyId: string;
  isSentByEmail: boolean;
}

export interface IPosition {
  id: number;
  name: string;
  descr: string;
  updated: Date;
  isActive: boolean;
  departmentId: number;
  hrCompaniesIds: number[];
  interviewersIds: number[];
  candidates: number[];
}

export interface ICvReview {
  candidateId: number;
  reviewHtml: string;
  reviewText: string;
}
