import { makeAutoObservable } from "mobx";
import CvsApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.cvsApi = new CvsApi();
  }

  //   async search() {
  //     const aaa = await this.cvsApi.search();
  //     console.log(aaa);
  //   }
}
