import { IIdName } from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class GeneralApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getFileBase64() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Download")).data;
      return data;
    });
  }

  async addUpdateHrCompany(hrCompany: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("General/AddUpdateHrCompany", hrCompany)
      ).data;
      return data;
    });

    return response;
  }

  async getHrCompaniesList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IIdName[]>("General/GetHrCompaniesList")
      ).data;
      return data;
    });

    return response;
  }

  async deleteHrCompany(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.delete(`General/DeleteHrCompany?id=${id}`))
        .data;
      return data;
    });

    return response;
  }
}
