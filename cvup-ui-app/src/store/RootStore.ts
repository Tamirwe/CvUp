import { IAppSettings } from "../models/GeneralModels";
import { AuthStore } from "./AuthStore";
import { CandsStore } from "./CandsStore";
import { GeneralStore } from "./GeneralStore";
import { PositionsStore } from "./PositionsStore";
import { ContactsStore } from "./ContactsStore";
import { FoldersStore } from "./FoldersStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;
  positionsStore: PositionsStore;
  contactsStore: ContactsStore;
  foldersStore: FoldersStore;
  candsStore: CandsStore;

  constructor(appSettings: IAppSettings) {
    this.authStore = new AuthStore(this, appSettings);
    this.generalStore = new GeneralStore(this, appSettings);
    this.positionsStore = new PositionsStore(this, appSettings);
    this.contactsStore = new ContactsStore(this, appSettings);
    this.foldersStore = new FoldersStore(this, appSettings);
    this.candsStore = new CandsStore(this, appSettings);
  }
}
