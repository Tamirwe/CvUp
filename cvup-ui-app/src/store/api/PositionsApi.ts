import { IPosition } from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class PositionsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async addUpdatePosition(position: IPosition) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Auth/AddUpdatePosition", position))
        .data;
      return data;
    });

    return response;
  }
}
