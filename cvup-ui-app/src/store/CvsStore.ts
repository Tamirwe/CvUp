import { makeAutoObservable, runInAction } from "mobx";
import { ICvListItem } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  cvsList: ICvListItem[] = [];
  cvId: string = "";

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.cvsApi = new CvsApi();
  }

  reset() {
    this.cvsList = [];
    this.cvId = "";
  }

  async getCv(cvId: string) {
    this.cvId = cvId;
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCv();

    this.rootStore.generalStore.backdrop = false;
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
