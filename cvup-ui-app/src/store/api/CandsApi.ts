import {
  ICand,
  ICvReview,
  ICompanyStagesTypes,
} from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CandsApi extends BaseApi {
  async search() {
    return await this.apiWrapper2(async () => {
      return await this.http.get("Search");
    });
  }

  async searchCands(value: string, posId?: number, folderId?: number) {
    return await this.apiWrapper2<ICand[]>(async () => {
      return await this.http.post(`Cand/SearchCands?searchKeyWords`, {
        keyWords: value,
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

  async GetPosCandsList(positionId: number) {
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
    return await this.apiWrapper2(async () => {
      return await this.http.post(`Cand/AttachPosCandCv`, {
        candidateId,
        cvId,
        positionId,
        keyId,
      });
    });
  }

  async detachPosCand(candidateId: number, cvId: number, positionId: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.post(`Cand/DetachPosCand`, {
        candidateId,
        cvId,
        positionId,
      });
    });
  }

  async detachFolderCand(folderCandId: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(
        `Folders/DetachCandidate?id=${folderCandId}`
      );
    });
  }

  async getCv() {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.get("Cand/getCv");
    });
  }

  async saveCvReview(cvReview: ICvReview) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Cand/SaveCvReview", cvReview);
    });
  }

  async saveCandReview(review: string, candidateId: number) {
    return await this.apiWrapper2<boolean>(async () => {
      return await this.http.put("Cand/SaveCandReview", {
        review,
        candidateId,
      });
    });
  }

  async getCompanyStagesTypes() {
    return await this.apiWrapper2<ICompanyStagesTypes[]>(async () => {
      return await this.http.get("Cand/GetCompanyStagesTypes");
    });
  }
}
