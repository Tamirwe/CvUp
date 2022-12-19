import { IIdName, IHrCompany } from "../../models/AuthModels";
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

  async addUpdateDepartment(department: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("General/AddUpdateDepartment", department)
      ).data;
      return data;
    });

    return response;
  }

  async getDepartments() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IIdName[]>("General/GetDepartments"))
        .data;
      return data;
    });

    return response;
  }

  async deleteDepartment(department: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete(
          `General/DeleteDepartment?id=${department.id}`
        )
      ).data;
      return data;
    });

    return response;
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

  async getHrCompanies() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IIdName[]>("General/GetHrCompanies"))
        .data;
      return data;
    });

    return response;
  }

  async deleteHrCompany(department: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete(
          `General/DeleteHrCompany?id=${department.id}`
        )
      ).data;
      return data;
    });

    return response;
  }
}
