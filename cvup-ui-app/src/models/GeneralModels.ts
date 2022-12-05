export interface SelectModel {
  id: number;
  name: string;
  isSelected?: boolean;
}
export interface ClaimsModel {
  CompanyId?: string;
  UserId?: string;
  DisplayName?: string;
  email?: string;
  role?: string;
}

export interface CvListItemModel {
  cvId: string;
  encriptedId: string;
  phone?: string;
  email?: string;
  emailSubject: string;
  candidateName?: string;
}

