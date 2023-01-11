import { IAppSettings } from "../models/GeneralModels";
import { AuthStore } from "./AuthStore";
import { CvsStore } from "./CvsStore";
import { GeneralStore } from "./GeneralStore";
import { PositionsStore } from "./PositionsStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;
  positionsStore: PositionsStore;
  cvsStore: CvsStore;

  constructor(appSettings: IAppSettings) {
    this.authStore = new AuthStore(this, appSettings);
    this.generalStore = new GeneralStore(this, appSettings);
    this.positionsStore = new PositionsStore(this, appSettings);
    this.cvsStore = new CvsStore(this, appSettings);
  }
}
