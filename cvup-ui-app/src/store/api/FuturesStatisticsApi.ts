import { Iohlc } from "../../models/FuStatModel";
import BaseApi from "./BaseApi";

export default class FuStatApi extends BaseApi {
  async getDayOhlcList() {
    return await this.apiWrapper2<Iohlc[]>(async () => {
      return await this.http.get("FuturesStatistics/GetDayOhlcList");
    });
  }

  async addDayOhlc(contactModel: Iohlc) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("FuturesStatistics/AddDayOhlc", contactModel);
    });
  }

  async updateDayOhlc(contactModel: Iohlc) {
    return await this.apiWrapper2(async () => {
      return await this.http.put(
        "FuturesStatistics/UpdateDayOhlc",
        contactModel
      );
    });
  }

  async deleteDayOhlc(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("FuturesStatistics/deleteDayOhlc?id=" + id);
    });
  }
}
