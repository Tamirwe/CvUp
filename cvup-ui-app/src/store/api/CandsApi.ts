import {
  ICand,
  IPosStagesType,
  IEmailTemplate,
  ISendEmail,
  ISearchModel,
  ICandsReport,
  ICandPosStageTypeUpdate,
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
    posTypeId?: number,
    folderId?: number
  ) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.post(`Cand/SearchCands?searchKeyWords`, {
        ...searchVals,
        positionId: posId,
        positionTypeId: posTypeId,
        folderId: folderId,
      });
    });
  }

  async getCandsList() {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get("Cand/GetCandsList");
    });
  }

  async getDuplicatesCvsList(candidateId: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get(
        `Cand/GetCandCvsList?candidateId=${candidateId}`
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

  async getPosTypeCandsList(posTypeId: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.get(
        `Cand/GetPosTypeCandsList?positionTypeId=${posTypeId}`
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

  async saveCustomerCandReview(
    review: string,
    candidateId: number,
    positionId?: number
  ) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.put("Cand/SaveCustomerCandReview", {
        review,
        candidateId,
        positionId,
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

  async getCandPosStages() {
    return await this.apiWrapper2<IPosStagesType[]>(async () => {
      return await this.http.get("Cand/GetCandPosStagesTypes");
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

  async updateCandPositionStatus(stageToUpdate: ICandPosStageTypeUpdate) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(
        `Cand/UpdateCandPositionStatus`,
        stageToUpdate
      );
    });
  }

  async updatePosStageDate(stageToUpdate: ICandPosStageTypeUpdate) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(`Cand/UpdatePosStageDate`, stageToUpdate);
    });
  }

  async removePosStage(stageToUpdate: ICandPosStageTypeUpdate) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post(`Cand/RemovePosStage`, stageToUpdate);
    });
  }

  async updateIsSeen(cvId: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.get(`Cand/UpdateIsSeen?cvId=${cvId}`);
    });
  }

  async getCandsReport(stageType: string) {
    return await this.apiWrapper2<ICandsReport[]>(async () => {
      return await this.http.get(`Cand/CandsReport?stageType=${stageType}`);
    });
  }

  async deleteCv(candidateId?: number, cvId?: number) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.delete(
        `Cand/DeleteCv?cnid=${candidateId}&cvId=${cvId}`
      );
    });
  }

  async deleteCandidate(candidateId?: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(`Cand/DeleteCandidate?id=${candidateId}`);
    });
  }

  async getfile(keyId: string) {
    return await this.apiWrapper2<Blob>(async () => {
      return await this.http.get(`DD/GetFileStream?id=${keyId}`, {
        responseType: "blob",
        timeout: 30000,
      });
    });
  }

  async getPdfFile(keyId: string) {
    return await this.apiWrapper2<Blob>(async () => {
      return await this.http.get(`DD?id=${keyId}`, {
        responseType: "blob",
        timeout: 30000,
      });
    });
  }

  async saveSearch(searchVals: ISearchModel) {
    return await this.apiWrapper2(async () => {
      return await this.http.post(`Cand/SaveSearch`, searchVals);
    });
  }

  async getSearches() {
    return await this.apiWrapper2<ISearchModel[]>(async () => {
      return await this.http.get(`Cand/GetSearches`);
    });
  }

  async starSearch(searchVals: ISearchModel) {
    return await this.apiWrapper2(async () => {
      return await this.http.put(`Cand/StarSearch`, searchVals);
    });
  }

  async deleteSearch(searchVals: ISearchModel) {
    return await this.apiWrapper2(async () => {
      return await this.http.put(`Cand/DeleteSearch`, searchVals);
    });
  }

  async deleteAllNotStarSearches() {
    return await this.apiWrapper2<ISearchModel[]>(async () => {
      return await this.http.put(`Cand/DeleteAllNotStarSearches`);
    });
  }
}
