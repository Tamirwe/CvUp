import { IIdName } from "../../models/AuthModels";
import { IContact } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CustomersContactsApi extends BaseApi {
  async addContact(contactModel: IContact) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("CustomersContacts/AddContact", contactModel)
      ).data;
      return response;
    });

    return responseData;
  }

  async updateContact(contactModel: IContact) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.put("CustomersContacts/UpdateContact", contactModel)
      ).data;
      return response;
    });

    return responseData;
  }

  async deleteContact(id: number) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.delete("CustomersContacts/DeleteContact?id=" + id)
      ).data;
      return response;
    });

    return responseData;
  }

  async getContactsList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.get<IContact[]>("CustomersContacts/GetContacts")
      ).data;
      return response;
    });

    return responseData;
  }

  async addCustomer(customer: IIdName) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("CustomersContacts/AddCustomer", customer)
      ).data;
      return response;
    });

    return responseData;
  }

  async updateCustomer(customer: IIdName) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.put("CustomersContacts/UpdateCustomer", customer)
      ).data;
      return response;
    });

    return responseData;
  }

  async getCustomersList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.get<IIdName[]>("CustomersContacts/GetCustomersList")
      ).data;
      return response;
    });

    return responseData;
  }

  async deleteCustomer(id: number) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.delete(`CustomersContacts/DeleteCustomer?id=${id}`)
      ).data;
      return response;
    });

    return responseData;
  }
}
