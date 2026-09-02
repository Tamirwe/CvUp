import { AuthStore } from "./AuthStore";
import { CandsStore } from "./CandsStore";
import { GeneralStore } from "./GeneralStore";
import { PositionsStore } from "./PositionsStore";
import { CustomersContactsStore } from "./CustomersContactsStore";
import { FoldersStore } from "./FoldersStore";
import { ScheduleInterviewStore } from "./ScheduleInterviewStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;
  positionsStore: PositionsStore;
  customersContactsStore: CustomersContactsStore;
  foldersStore: FoldersStore;
  candsStore: CandsStore;
  scheduleInterviewStore: ScheduleInterviewStore;

  constructor() {
    this.authStore = new AuthStore(this);
    this.generalStore = new GeneralStore(this);
    this.positionsStore = new PositionsStore(this);
    this.customersContactsStore = new CustomersContactsStore(this);
    this.foldersStore = new FoldersStore(this);
    this.candsStore = new CandsStore(this);
    this.scheduleInterviewStore = new ScheduleInterviewStore(this);
  }
}
