import { ICand, ICvReview } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CandsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getCandsList() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICand[]>("Cand/GetCandsList")).data;
      return data;
    });
  }

  async getDuplicatesCvsList(cvId: number, candidateId: number) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.get<ICand[]>(
          `Cand/GetCandCvsList?cvId=${cvId}&candidateId=${candidateId}`
        )
      ).data;
      return data;
    });
  }

  async GetPosCandsList(positionId: number) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.get<ICand[]>(
          `Cand/GetPosCandsList?positionId=${positionId}`
        )
      ).data;
      return data;
    });
  }

  async attachPosCandCv(
    candidateId: number,
    cvId: number,
    positionId: number,
    keyId: string
  ) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.post(`Cand/AttachPosCandCv`, {
          candidateId,
          cvId,
          positionId,
          keyId,
        })
      ).data;
      return data;
    });
  }

  async detachPosCandidate(
    candidateId: number,
    cvId: number,
    positionId: number
  ) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.post(`Cand/DetachPosCandidate`, {
          candidateId,
          cvId,
          positionId,
        })
      ).data;
      return data;
    });
  }

  async getCv() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICand>("Cand/getCv")).data;
      return data;
    });
  }

  async saveCvReview(cvReview: ICvReview) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Cand/SaveCvReview", cvReview)).data;
      return data;
    });

    return response;
  }
}
