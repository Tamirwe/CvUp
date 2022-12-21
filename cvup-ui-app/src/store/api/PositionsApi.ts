import { IPosition, IPositionListItem } from "../../models/GeneralModels";
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
      const data = (await this.http.post("Positions/AddUpdatePosition", position))
        .data;
      return data;
    });

    return response;
  }

  async getPositionsList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IPositionListItem[]>("Positions/GetPositionsList")
      ).data;
      return data;
    });

    return response;
  }

  async deletePosition(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.delete(`Positions/DeletePosition?id=${id}`))
        .data;
      return data;
    });

    return response;
  }
}
