import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import { IAppSettings, IContact } from "../models/GeneralModels";
import ContactsApi from "./api/CustomersContactsApi";
import { RootStore } from "./RootStore";

export class CustomersContactsStore {
  private contactsApi;
  private contactSelected?: IContact;
  private customerSelected?: IIdName;
  contactsList: IContact[] = [];
  customersList: IIdName[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.contactsApi = new ContactsApi(appSettings);
  }

  reset() {}

  get selectedContact() {
    return this.contactSelected;
  }

  set selectedContact(val: IContact | undefined) {
    this.contactSelected = val;
  }

  get selectedCustomer() {
    return this.customerSelected;
  }

  set selectedCustomer(val: IIdName | undefined) {
    this.customerSelected = val;
  }

  async addContact(contactModel: IContact) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.addContact(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async updateContact(contactModel: IContact) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.updateContact(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async deleteContact(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.deleteContact(id);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getContactsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.contactsApi.getContactsList();
    runInAction(() => {
      this.contactsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getCustomersList(loadAgain: boolean) {
    this.rootStore.generalStore.backdrop = true;
    if (this.customersList.length === 0 || loadAgain) {
      const res = await this.contactsApi.getCustomersList();
      runInAction(() => {
        this.customersList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async addCustomer(customer: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.addCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async updateCustomer(customer: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.updateCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async deleteCustomer(customerId: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.contactsApi.deleteCustomer(customerId);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }
}
