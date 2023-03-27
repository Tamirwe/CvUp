import { ICand, ICvReview } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CandsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.get("Search")).data;
      return response;
    });
  }

  async searchCands(value: string) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.get<ICand[]>(`Cand/SearchCands?searchKeyWords=${value}`)
      ).data;
      return response;
    });
  }

  async getCandsList() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.get<ICand[]>("Cand/GetCandsList")).data;
      return response;
    });
  }

  async getDuplicatesCvsList(cvId: number, candidateId: number) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.get<ICand[]>(
          `Cand/GetCandCvsList?cvId=${cvId}&candidateId=${candidateId}`
        )
      ).data;
      return response;
    });
  }

  async GetPosCandsList(positionId: number) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.get<ICand[]>(
          `Cand/GetPosCandsList?positionId=${positionId}`
        )
      ).data;
      return response;
    });
  }

  async getFolderCandsList(folderId: number) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.get<ICand[]>(
          `Cand/GetFolderCandsList?folderId=${folderId}`
        )
      ).data;
      return response;
    });
  }

  async attachPosCandCv(
    candidateId: number,
    cvId: number,
    positionId: number,
    keyId: string
  ) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.post(`Cand/AttachPosCandCv`, {
          candidateId,
          cvId,
          positionId,
          keyId,
        })
      ).data;
      return response;
    });
  }

  async detachPosCand(candidateId: number, cvId: number, positionId: number) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.post(`Cand/DetachPosCand`, {
          candidateId,
          cvId,
          positionId,
        })
      ).data;
      return response;
    });
  }

  async detachFolderCand(folderCandId: number) {
    return await this.apiWrapper(async () => {
      const response = (
        await this.http.delete(`Folders/DetachCandidate?id=${folderCandId}`)
      ).data;
      return response;
    });
  }

  async getCv() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.get<ICand>("Cand/getCv")).data;
      return response;
    });
  }

  async saveCvReview(cvReview: ICvReview) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.post("Cand/SaveCvReview", cvReview))
        .data;
      return response;
    });

    return responseData;
  }
}
