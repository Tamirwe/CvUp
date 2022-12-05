import { CvListItemModel } from "../../models/GeneralModels";
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
      const data = (await this.http.get<CvListItemModel[]>("Cvs/getCvsList"))
        .data;
      return data;
    });
  }
}
