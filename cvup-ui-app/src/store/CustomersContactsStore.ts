import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import { IAppSettings, IContact } from "../models/GeneralModels";
import ContactsApi from "./api/CustomersContactsApi";
import { RootStore } from "./RootStore";

export class CustomersContactsStore {
  private contactsApi;
  private contactSelected?: IContact;
  private customerSelected?: IIdName;
  private contactsList: IContact[] = [];
  private searchPhrase: string = "";
  customersList: IIdName[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.contactsApi = new ContactsApi(appSettings);
  }

  reset() {}

  get contactsListSorted() {
    if (this.searchPhrase) {
      return this.contactsList.filter((x) =>
        this.searchStringContacts(x).includes(this.searchPhrase.toLowerCase())
      );
    } else {
      return this.contactsList.slice();
    }
  }

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

  searchContacts(val: string) {
    this.searchPhrase = val;
  }

  searchStringContacts = (x: IContact) => {
    return (
      (x.firstName + "").toLowerCase() +
      (x.lastName + "").toLowerCase() +
      (x.customerName + "").toLowerCase()
    );
  };

  async addContact(contactModel: IContact) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.addContact(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateContact(contactModel: IContact) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.updateContact(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteContact(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.deleteContact(id);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async getContactsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.contactsApi.getContactsList();
    runInAction(() => {
      this.contactsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getCustomersList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.contactsApi.getCustomersList();
    runInAction(() => {
      this.customersList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async addCustomer(customer: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.addCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateCustomer(customer: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.updateCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteCustomer(customerId: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.deleteCustomer(customerId);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
