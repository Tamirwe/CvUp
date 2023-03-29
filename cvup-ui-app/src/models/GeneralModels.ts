import { UserRoleEnum } from "./GeneralEnums";

export interface IAppSettings {
  appServerUrl: string;
  appMode: string;
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
  role?: UserRoleEnum;
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
  folderCandId: number;
}

export interface ICandCv {
  cvId: number;
  candidateId: number;
  keyId: string;
  emailSubject: string;
  fileType: string;
  cvSent: Date;
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
  updated?: Date;
  isActive: boolean;
  customerId: number;
  hrCompaniesIds: number[];
  interviewersIds: number[];
  // candidates: number[];
}

export interface ICvReview {
  candidateId: number;
  reviewHtml: string;
  reviewText: string;
}

export interface IMailsList {
  email: string;
  name: string;
  userTyped?: boolean;
}

export interface IContact {
  id: number;
  firstName: string;
  lastName: string;
  customerId: number;
  customerName?: string;
  email: string;
  phone: string;
}

export interface IFolder {
  id: number;
  name: string;
  parentId: number;
}

export interface IFolderNode {
  folder: IFolder;
  children: IFolder[];
}
