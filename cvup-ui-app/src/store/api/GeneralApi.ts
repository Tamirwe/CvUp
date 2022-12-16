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

  async addUpdateHrCompany(hrCompany: IHrCompany) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("General/AddUpdateHrCompany", hrCompany)
      ).data;
      return data;
    });

    return response;
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

  async deleteDepartment(department: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete<IIdName>(
          `General/DeleteCompanyDepartment?id=${department.id}`
        )
      ).data;
      return data;
    });

    return response;
  }

  async getCompanyDepartments() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IIdName[]>("General/GetCompanyDepartments")
      ).data;
      return data;
    });

    return response;
  }
}
