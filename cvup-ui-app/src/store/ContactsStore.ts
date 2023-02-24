import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, IContact } from "../models/GeneralModels";
import ContactsApi from "./api/ContactsApi";
import { RootStore } from "./RootStore";

export class ContactsStore {
  private contactsApi;
  contactsList: IContact[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.contactsApi = new ContactsApi(appSettings);
  }

  reset() {}

  async newContact() {
    runInAction(() => {});
  }

  async getContactsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.contactsApi.getContactsList();
    runInAction(() => {
      this.contactsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }
}
