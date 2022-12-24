import { makeAutoObservable, runInAction } from "mobx";
import { ICvListItemModel } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  cvsList: ICvListItemModel[] = [];
  cvId: string = "";

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.cvsApi = new CvsApi();
  }

  reset() {
    this.cvsList = [];
    this.cvId = "";
  }

  async getCvsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCvsList();
    runInAction(() => {
      this.cvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  setDoc(cvId: string) {
    runInAction(() => {
      this.cvId = cvId;
    });
  }
  //   async search() {
  //     const aaa = await this.cvsApi.search();
  //     console.log(aaa);
  //   }
}
