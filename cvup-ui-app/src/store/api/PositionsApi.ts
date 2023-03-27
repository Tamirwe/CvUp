import { IPosition } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class PositionsApi extends BaseApi {
  async search() {
    return await this.apiWrapper2(async () => {
      return await this.http.get("Search");
    });
  }

  async getPosition(positionId: number) {
    return await this.apiWrapper2<IPosition>(async () => {
      return await this.http.get(`Positions/GetPosition?id=${positionId}`);
    });
  }

  async addUpdatePosition(position: IPosition) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.post("Positions/AddUpdatePosition", position);
    });
  }

  async getPositionsList() {
    return await this.apiWrapper2<IPosition[]>(async () => {
      return await this.http.get("Positions/GetPositionsList");
    });
  }

  async deletePosition(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(`Positions/DeletePosition?id=${id}`);
    });
  }
}
