export enum CrudTypesEnum {
  Insert,
  Update,
  Delete,
  None,
}

export enum PermissionTypeEnum {
  Admin,
  User,
}

export enum UserActiveEnum {
  Active,
  Wait_Complete_Registration,
  Not_Active,
}

export enum PositionStatusEnum {
  Active,
  Not_Active,
  Completed,
}

export enum CvDisplayedListEnum {
  None,
  CandsList,
  PositionCandsList,
  FolderCandsList,
}

export enum EmailTypeEnum {
  None,
  Candidate,
  Contact,
}

export enum TabsGeneralEnum {
  Contacts,
  Folders,
  Types,
  Positions,
}

export enum TabsCandsEnum {
  None,
  AllCands,
  PositionCands,
  FolderCands,
  PositionTypeCands,
}

export enum TextValidateTypeEnum {
  notEmpty,
  twoCharsMin,
  startWithTwoLetters,
  onlyLetters,
  emailValid,
  phoneValid,
  notSelected,
}

export enum UserRoleEnum {
  Admin,
  User,
}

export enum AlertConfirmDialogEnum {
  Alert,
  Confirm,
}

export enum AppModeEnum {
  HRCompany,
  HRDepartment,
}

export enum CandsSourceEnum {
  AllCands,
  Position,
  PositionType,
  Folder,
}

export enum DynamicEmailDataEnum {
  FirstName,
  FullName,
  CustomerName,
  PositionName,
  MyFirstName,
  MyFullname,
  MyPhoneNumber,
  MyEmail,
  MyFirstNameEn,
  MyFullnameEn,
}

export enum SortByEnum {
  score,
  cvDate,
}
