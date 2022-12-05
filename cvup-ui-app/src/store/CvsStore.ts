import { makeAutoObservable, runInAction } from "mobx";
import { CvListItemModel } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  cvsList: CvListItemModel[] = [];
  docIdEncript: string = "http://89.237.94.86:8010/api/Download/GetWord2";

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

  setDoc(docIdEncript: string) {
    this.docIdEncript = docIdEncript;
  }
  //   async search() {
  //     const aaa = await this.cvsApi.search();
  //     console.log(aaa);
  //   }
}
