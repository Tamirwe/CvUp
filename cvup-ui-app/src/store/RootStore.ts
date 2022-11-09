import { AuthStore } from "./AuthStore";
import { GeneralStore } from "./GeneralStore";

export class RootStore {
  authStore: AuthStore;
  generalStore: GeneralStore;

  constructor() {
    this.authStore = new AuthStore(this);
    this.generalStore = new GeneralStore(this);
  }
}
