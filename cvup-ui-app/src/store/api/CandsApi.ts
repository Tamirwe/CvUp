import {
  ICand,
  ICvReview,
  ICompanyStagesTypes,
  IEmailTemplate,
  IEmailsAddress,
  IAttachCv,
  ISendEmail,
  ISearchModel,
} from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CandsApi extends BaseApi {
  async search() {
    return await this.apiWrapper2(async () => {
      return await this.http.get("Search");
    });
  }

  async searchCands(
    searchVals: ISearchModel,
    posId?: number,
    folderId?: number
  ) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.post(`Cand/SearchCands?searchKeyWords`, {
        ...searchVals,
        positionId: posId,
        folderId: folderId,
      });
    });
  }

  async getCandsList() {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get("Cand/GetCandsList");
    });
  }

  async getDuplicatesCvsList(cvId: number, candidateId: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get(
        `Cand/GetCandCvsList?cvId=${cvId}&candidateId=${candidateId}`
      );
    });
  }

  async getPosCandsList(positionId: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get(
        `Cand/GetPosCandsList?positionId=${positionId}`
      );
    });
  }

  async getFolderCandsList(folderId: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get(
        `Cand/GetFolderCandsList?folderId=${folderId}`
      );
    });
  }

  async attachPosCandCv(
    candidateId: number,
    cvId: number,
    positionId: number,
    keyId: string
  ) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(`Cand/AttachPosCandCv`, {
        candidateId,
        cvId,
        positionId,
        keyId,
      });
    });
  }

  async detachPosCand(candidateId: number, cvId: number, positionId: number) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(`Cand/DetachPosCand`, {
        candidateId,
        cvId,
        positionId,
      });
    });
  }

  async getCv() {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.get("Cand/getCv");
    });
  }

  async saveCandReview(review: string, candidateId: number) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.put("Cand/SaveCandReview", {
        review,
        candidateId,
      });
    });
  }

  async saveCandDetails(
    candidateId: number,
    firstName: string,
    lastName: string,
    email: string,
    phone: string
  ) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.put("Cand/UpdateCandDetails", {
        candidateId,
        firstName,
        lastName,
        email,
        phone,
      });
    });
  }

  async getCompanyStagesTypes() {
    return await this.apiWrapper2<ICompanyStagesTypes[]>(async () => {
      return await this.http.get("Cand/GetCompanyStagesTypes");
    });
  }

  async getEmailTemplates() {
    return await this.apiWrapper2<IEmailTemplate[]>(async () => {
      return await this.http.get("Cand/GetEmailTemplates");
    });
  }

  async addUpdateEmailTemplate(emailTemplate: IEmailTemplate) {
    return await this.apiWrapper2(async () => {
      return await this.http.post(`Cand/AddUpdateEmailTemplate`, emailTemplate);
    });
  }

  async deleteEmailTemplate(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(`Cand/DeleteEmailTemplate?id=${id}`);
    });
  }

  async sendEmail(emailData: ISendEmail) {
    return await this.apiWrapper2(async () => {
      return await this.http.post(`Cand/SendEmail`, emailData);
    });
  }

  async updateCandPositionStatus(
    stageType: string,
    candidateId: number,
    positionId: number
  ) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(`Cand/UpdateCandPositionStatus`, {
        stageType,
        candidateId,
        positionId,
      });
    });
  }

  updateIsSeen(cvId: number) {
    return this.apiWrapper2<ICand>(async () => {
      return this.http.put(`Cand/UpdateIsSeen`, {
        cvId,
      });
    });
  }
}
