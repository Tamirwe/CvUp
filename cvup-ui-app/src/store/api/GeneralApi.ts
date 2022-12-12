import { IDepartment, IHrCompany } from "../../models/AuthModels";
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

  async addUpdateHrCompany(hrCompany: IHrCompany) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Auth/AddUpdateHrCompany", hrCompany))
        .data;
      return data;
    });

    return response;
  }

  async addUpdateDepartment(department: IDepartment) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("Auth/AddUpdateDepartment", department)
      ).data;
      return data;
    });

    return response;
  }
}
