import { IAppSettings } from "../models/GeneralModels";
import { AuthStore } from "./AuthStore";
import { CandsStore } from "./CandsStore";
import { GeneralStore } from "./GeneralStore";
import { PositionsStore } from "./PositionsStore";
import { CustomersContactsStore } from "./CustomersContactsStore";
import { FoldersStore } from "./FoldersStore";
import { FuturesStatisticStore } from "./FuturesStatisticsStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;
  positionsStore: PositionsStore;
  customersContactsStore: CustomersContactsStore;
  foldersStore: FoldersStore;
  candsStore: CandsStore;
  futuresStatisticStore: FuturesStatisticStore;

  constructor(appSettings: IAppSettings) {
    this.authStore = new AuthStore(this, appSettings);
    this.generalStore = new GeneralStore(this, appSettings);
    this.positionsStore = new PositionsStore(this, appSettings);
    this.customersContactsStore = new CustomersContactsStore(this, appSettings);
    this.foldersStore = new FoldersStore(this, appSettings);
    this.candsStore = new CandsStore(this, appSettings);
    this.futuresStatisticStore = new FuturesStatisticStore(this, appSettings);
  }
}
