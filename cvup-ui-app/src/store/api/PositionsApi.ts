import { IPosition, IPositionType } from "../../models/GeneralModels";
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

  async getPositionContactsIds(positionId: number) {
    return await this.apiWrapper2<number[]>(async () => {
      return await this.http.get(
        `Positions/getPositionContactsIds?posId=${positionId}`
      );
    });
  }

  async addPosition(position: IPosition) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.post<number>("Positions/AddPosition", position);
    });
  }

  async updatePosition(position: IPosition) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.put<number>("Positions/UpdatePosition", position);
    });
  }

  async getPositionsList() {
    return await this.apiWrapper2<IPosition[]>(async () => {
      return await this.http.get("Positions/GetPositionsList");
    });
  }

  async getPositionsTypesList() {
    return await this.apiWrapper2<IPositionType[]>(async () => {
      return await this.http.get("Positions/GetPositionsTypes");
    });
  }

  async deletePosition(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(`Positions/DeletePosition?id=${id}`);
    });
  }
}
