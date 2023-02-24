import { IContact } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class ContactsApi extends BaseApi {
  async getContactsList() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IContact[]>("Contacts/GetContactsList"))
        .data;
      return data;
    });

    return response;
  }
}
