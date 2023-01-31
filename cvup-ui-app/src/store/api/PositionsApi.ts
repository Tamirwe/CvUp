import { IPosition } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class PositionsApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getPosition(positionId: number) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IPosition>(`Positions/GetPosition?id=${positionId}`)
      ).data;
      return data;
    });

    return response;
  }

  async addUpdatePosition(position: IPosition) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post<number>("Positions/AddUpdatePosition", position)
      ).data;
      return data;
    });

    return response;
  }

  async getPositionsList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IPosition[]>("Positions/GetPositionsList")
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
