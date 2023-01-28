import { ICv, ICvReview } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CvsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getCvsList() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICv[]>("Cvs/getCvsList")).data;
      return data;
    });
  }

  async GetDuplicatesCvsList(cvId: number, candidateId: number) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.get<ICv[]>(
          `Cvs/GetDuplicatesCvsList?cvId=${cvId}&candidateId=${candidateId}`
        )
      ).data;
      return data;
    });
  }

  async AttachePosCv(
    candidateId: number,
    cvId: number,
    posId: number,
    candPosIds: number[],
    cvPosIds: number[],
    isAttach: boolean
  ) {
    return await this.apiWrapper(async () => {
      const data = (
        await this.http.post(`Cvs/AttachePosCv`, {
          candidateId,
          cvId,
          posId,
          candPosIds,
          cvPosIds,
          isAttach,
        })
      ).data;
      return data;
    });
  }

  async getCv() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICv>("Cvs/getCv")).data;
      return data;
    });
  }

  async saveCvReview(cvReview: ICvReview) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Cvs/SaveCvReview", cvReview)).data;
      return data;
    });

    return response;
  }
}
