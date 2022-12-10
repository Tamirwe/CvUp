import { makeAutoObservable, runInAction } from "mobx";
import { CvListItemModel } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  cvsList: CvListItemModel[] = [];
  cvId: string = "";

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.cvsApi = new CvsApi();
  }

  async getCvsList() {
    const res = await this.cvsApi.getCvsList();
    runInAction(() => {
      this.cvsList = res.data;
    });
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
