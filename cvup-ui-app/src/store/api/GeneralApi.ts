import { IIdName } from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class GeneralApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.get("Search")).data;
      return response;
    });
  }

  async getFileBase64() {
    return await this.apiWrapper(async () => {
      const response = (await this.http.get("Download")).data;
      return response;
    });
  }

  async addUpdateHrCompany(hrCompany: IIdName) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("General/AddUpdateHrCompany", hrCompany)
      ).data;
      return response;
    });

    return responseData;
  }

  async getHrCompaniesList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.get<IIdName[]>("General/GetHrCompaniesList")
      ).data;
      return response;
    });

    return responseData;
  }

  async deleteHrCompany(id: number) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.delete(`General/DeleteHrCompany?id=${id}`)
      ).data;
      return response;
    });

    return responseData;
  }
}
