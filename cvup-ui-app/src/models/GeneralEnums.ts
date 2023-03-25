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
  Not_Active,
  Wait_Complete_Registration,
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
  Positions,
}

export enum TabsCandsEnum {
  AllCands,
  PositionCands,
  FolderCands,
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
