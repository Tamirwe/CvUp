import { ICv, ICvListItem } from "../../models/GeneralModels";
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
      const data = (await this.http.get<ICvListItem[]>("Cvs/getCvsList")).data;
      return data;
    });
  }

  async getCv() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get<ICv>("Cvs/getCv")).data;
      return data;
    });
  }
}
