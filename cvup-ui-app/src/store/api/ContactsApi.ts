import { IContact } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class ContactsApi extends BaseApi {
  async addContact(contactModel: IContact) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Contacts/AddContact", contactModel))
        .data;
      return data;
    });

    return response;
  }

  async updateContact(contactModel: IContact) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("Contacts/UpdateContact", contactModel)
      ).data;
      return data;
    });

    return response;
  }

  async deleteContact(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.delete("Contacts/DeleteContact?id=" + id))
        .data;
      return data;
    });

    return response;
  }

  async getContactsList() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IContact[]>("Contacts/GetContactsList"))
        .data;
      return data;
    });

    return response;
  }
}
