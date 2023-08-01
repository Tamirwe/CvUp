import { PositionStatusEnum, UserRoleEnum } from "./GeneralEnums";

export interface IApiUrl {
  apiUrl: string;
}

export interface IAppSettingsFile {
  servers: IApiUrl[];
  appMode: string;
}

export interface IAppSettings {
  apiUrl: string;
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
  firstName?: string;
  lastName?: string;
  reviewTct: string;
  fileType: string;
  cvSent: Date;
  candFoldersIds: number[];
  candPosIds: number[];
  cvPosIds: number[];
  hasDuplicates: boolean;
  posStages?: IPosStages[];
  isSeen: boolean;
  reviewDate?: Date;
}

export interface IPosStages {
  id: number;
  t: string;
  d: string;
}

export interface ICompanyStagesTypes {
  stageType: string;
  order: number;
  name: string;
  color?: string;
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
  requirements: string;
  updated: Date;
  status: PositionStatusEnum;
  customerId: number;
  customerName: string;
  interviewersIds: number[];
  contactsIds: number[];
  emailsubjectAddon: string;
}

export interface ICvReview {
  candidateId: number;
  reviewHtml: string;
  reviewText: string;
}

export interface IEmailsAddress {
  Name: string;
  Address: string;
  userTyped?: boolean;
}

export interface IEmailForm {
  subject: string;
  body: string;
}

export interface IEmailTemplate {
  id: number;
  name: string;
  subject: string;
  body: string;
}

export interface ISendEmail {
  toAddresses: IEmailsAddress[];
  subject: string;
  body: string;
  attachCvs?: IAttachCv[];
  // candId?: number;
  // cvId?: number;
  // positionId?: number;
  // customerId?: number;
  // positionName?: string;
  // customerName?: string;
}

export interface IAttachCv {
  cvKey: string;
  name: string;
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

export interface ISearchModel {
  value: string;
  exact: boolean;
  advancedValue: string;
}
