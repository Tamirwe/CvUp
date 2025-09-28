import { Iohlc } from "../../models/FuStatModel";
import BaseApi from "./BaseApi";

export default class FuStatApi extends BaseApi {
  async getDailyStatList() {
    return await this.apiWrapper2<Iohlc[]>(async () => {
      return await this.http.get("CustomersContacts/GetContacts");
    });
  }

  async addDailyStat(contactModel: Iohlc) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("CustomersContacts/AddContact", contactModel);
    });
  }

  async updateDailyStat(contactModel: Iohlc) {
    return await this.apiWrapper2(async () => {
      return await this.http.put(
        "CustomersContacts/UpdateContact",
        contactModel
      );
    });
  }

  async deleteDailyStat(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("CustomersContacts/DeleteContact?id=" + id);
    });
  }
}
