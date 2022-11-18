import { makeAutoObservable } from "mobx";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi();
  }

  //   async search() {
  //     const aaa = await this.positionApi.search();
  //     console.log(aaa);
  //   }
}
