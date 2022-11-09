import { makeAutoObservable } from "mobx";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class GeneralStore {
  private generalApi;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.generalApi = new GeneralApi();
  }

  async search() {
    const aaa = await this.generalApi.search();
    console.log(aaa);
  }
}
