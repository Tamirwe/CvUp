import { IIdName } from "../../models/AuthModels";
import { IContact } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CustomersContactsApi extends BaseApi {
  async addContact(contactModel: IContact) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("CustomersContacts/AddContact", contactModel)
      ).data;
      return data;
    });

    return response;
  }

  async updateContact(contactModel: IContact) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.put("CustomersContacts/UpdateContact", contactModel)
      ).data;
      return data;
    });

    return response;
  }

  async deleteContact(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete("CustomersContacts/DeleteContact?id=" + id)
      ).data;
      return data;
    });

    return response;
  }

  async getContactsList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IContact[]>("CustomersContacts/GetContacts")
      ).data;
      return data;
    });

    return response;
  }

  async addCustomer(customer: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.post("CustomersContacts/AddCustomer", customer)
      ).data;
      return data;
    });

    return response;
  }

  async updateCustomer(customer: IIdName) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.put("CustomersContacts/UpdateCustomer", customer)
      ).data;
      return data;
    });

    return response;
  }

  async getCustomersList() {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.get<IIdName[]>("CustomersContacts/GetCustomersList")
      ).data;
      return data;
    });

    return response;
  }

  async deleteCustomer(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (
        await this.http.delete(`CustomersContacts/DeleteCustomer?id=${id}`)
      ).data;
      return data;
    });

    return response;
  }
}
