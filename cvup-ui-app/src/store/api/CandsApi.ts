import { ICv, ICvReview } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CandsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getCvsList() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICv[]>("Cand/GetCandList")).data;
      return data;
    });
  }

  async getDuplicatesCvsList(cvId: number, candidateId: number) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.get<ICv[]>(
          `Cand/GetDuplicatesCvsList?cvId=${cvId}&candidateId=${candidateId}`
        )
      ).data;
      return data;
    });
  }

  async getPosCvsList(positionId: number) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.get<ICv[]>(
          `Cand/GetPosCvsList?positionId=${positionId}`
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
      const data = (await this.http.get<ICv>("Cand/getCv")).data;
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
