import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, ICv, ICvReview } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  private isCvReviewDialogOpen: boolean = false;
  cvsList: ICv[] = [];
  keyId: string = "";
  cvSelected?: ICv;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CvsApi(appSettings);
  }

  reset() {
    this.cvsList = [];
    // this.cvId = "";
    this.cvSelected = undefined;
  }

  set openCvReviewDialogOpen(val) {
    this.isCvReviewDialogOpen = val;
  }

  get openCvReviewDialogOpen() {
    return this.isCvReviewDialogOpen;
  }

  async getCv(cv: ICv) {
    this.cvSelected = { ...cv };
    runInAction(() => {
      this.keyId = cv.keyId;
    });

    // this.rootStore.generalStore.backdrop = true;
    // const res = await this.cvsApi.getCv();

    // this.rootStore.generalStore.backdrop = false;
  }

  async saveCvReview(reviewText: any, reviewHtml: any) {
    // const cvReview:ICvReview = {  candidateId:};
    // const res = await this.cvsApi.saveCvReview(cvReview);
  }

  async getCvsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCvsList();
    runInAction(() => {
      this.cvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  // setDoc(cvId: string) {
  //   runInAction(() => {
  //     this.cvId = cvId;
  //   });
  // }
  //   async search() {
  //     const aaa = await this.cvsApi.search();
  //     console.log(aaa);
  //   }
}
