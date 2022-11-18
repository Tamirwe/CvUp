import { AuthStore } from "./AuthStore";
import { CvsStore } from "./CvsStore";
import { GeneralStore } from "./GeneralStore";
import { PositionsStore } from "./PositionsStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;
  positionsStore: PositionsStore;
  cvsStore: CvsStore;

  constructor() {
    this.authStore = new AuthStore(this);
    this.generalStore = new GeneralStore(this);
    this.positionsStore = new PositionsStore(this);
    this.cvsStore = new CvsStore(this);
  }
}
