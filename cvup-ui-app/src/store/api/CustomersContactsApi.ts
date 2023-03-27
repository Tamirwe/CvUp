import { IIdName } from "../../models/AuthModels";
import { IContact } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CustomersContactsApi extends BaseApi {
  async addContact(contactModel: IContact) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("CustomersContacts/AddContact", contactModel);
    });
  }

  async updateContact(contactModel: IContact) {
    return await this.apiWrapper2(async () => {
      return await this.http.put(
        "CustomersContacts/UpdateContact",
        contactModel
      );
    });
  }

  async deleteContact(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("CustomersContacts/DeleteContact?id=" + id);
    });
  }

  async getContactsList() {
    return await this.apiWrapper2<IContact[]>(async () => {
      return await this.http.get("CustomersContacts/GetContacts");
    });
  }

  async addCustomer(customer: IIdName) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("CustomersContacts/AddCustomer", customer);
    });
  }

  async updateCustomer(customer: IIdName) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("CustomersContacts/UpdateCustomer", customer);
    });
  }

  async getCustomersList() {
    return await this.apiWrapper2<IIdName[]>(async () => {
      return await this.http.get(
        "CustomersContacts/GetCustomersList"
      );
    });
  }

  async deleteCustomer(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(
        `CustomersContacts/DeleteCustomer?id=${id}`
      );
    });
  }
}
