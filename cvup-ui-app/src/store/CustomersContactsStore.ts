import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import {
  IAppSettings,
  IContact,
  ICustomer,
  ISearchModel,
} from "../models/GeneralModels";
import ContactsApi from "./api/CustomersContactsApi";
import { RootStore } from "./RootStore";

export class CustomersContactsStore {
  private contactsApi;
  private contactSelected?: IContact;
  private customerSelected?: IIdName;
  private searchPhrase?: ISearchModel;
  private isContactsListSortDesc: boolean = true;

  customersList: IIdName[] = [];
  customerAddedUpdated?: IIdName;
  contactsList: IContact[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.contactsApi = new ContactsApi(appSettings);
  }

  reset() {}

  get contactsListSorted() {
    let posList;

    if (this.searchPhrase && this.searchPhrase.value) {
      posList = this.contactsList.filter((x) =>
        this.searchStringContacts(x).includes(
          this.searchPhrase!.value.toLowerCase()
        )
      );
    } else {
      posList = this.contactsList.slice();
    }

    if (this.isContactsListSortDesc) {
      posList.sort((a, b) =>
        a.firstName > b.firstName ? 1 : b.firstName > a.firstName ? -1 : 0
      );
    } else {
      posList.sort((a, b) =>
        b.firstName > a.firstName ? 1 : a.firstName > b.firstName ? -1 : 0
      );
    }

    return posList;
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

  set contactsListSortDesc(val: boolean) {
    this.isContactsListSortDesc = val;
  }

  searchContacts(searchVals: ISearchModel) {
    this.searchPhrase = searchVals;
  }

  searchStringContacts = (x: IContact) => {
    return (
      (x.firstName + "").toLowerCase() +
      (x.lastName + "").toLowerCase() +
      (x.customerName + "").toLowerCase()
    );
  };

  getContactById = (id: number) => {
    return { ...this.contactsList.find((x) => x.id === id) };
  };

  getContactsByCustomerId = (id: number) => {
    return this.contactsList.filter((x) => x.customerId === id);
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

  async addCustomer(customer: ICustomer) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.contactsApi.addCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateCustomer(customer: ICustomer) {
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

  findCustomerName(customerId: number) {
    const customer = this.customersList.find((x) => x.id === customerId);
    if (customer) {
      return customer?.name;
    }

    return "";
  }

  setCustomerAddedUpdated(data?: IIdName) {
    runInAction(() => {
      this.customerAddedUpdated = data;
    });
  }
}
