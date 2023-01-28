import { makeAutoObservable, runInAction, toJS } from "mobx";
import { DisplayedCvsListEnum } from "../models/GeneralEnums";
import { IAppSettings, ICv, ICvReview } from "../models/GeneralModels";
import CvsApi from "./api/CvsApi";
import { RootStore } from "./RootStore";

export class CvsStore {
  private cvsApi;
  private isCvReviewDialogOpen: boolean = false;
  cvsList: ICv[] = [];
  duplicatesCvsList: ICv[] = [];
  pdfUrl: string = "";
  cvSelected?: ICv;
  cvDuplicateSelected?: ICv;
  cvDisplayed?: ICv;
  cvDisplayedList: DisplayedCvsListEnum = DisplayedCvsListEnum.None;

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
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
    runInAction(() => {
      this.cvSelected = cv;
      this.cvDisplayed = this.cvSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.SearchCvsList;
      this.getPdf();
    });
  }

  async getCvDuplicate(cv: ICv) {
    runInAction(() => {
      this.cvDuplicateSelected = cv;
      this.cvDisplayed = this.cvDuplicateSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.DuplicateCvsList;
      this.getPdf();
    });
  }

  async getPdf() {
    this.pdfUrl = `${this.appSettings.appServerUrl}DD?id=${this.cvDisplayed?.keyId}`;
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

  async GetDuplicatesCvsList(cv: ICv) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.GetDuplicatesCvsList(cv.cvId, cv.candidateId);
    runInAction(() => {
      this.duplicatesCvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async attachCvToPos(posId: number, checked: boolean) {
    const cv = toJS(this.cvDisplayed);
    let candPosIds: number[] = [];
    let cvPosIds: number[] = [];

    if (cv && cv.candPosIds) {
      if (checked) {
        cv.candPosIds.push(posId);
        cv.cvPosIds.push(posId);
      } else {
        let posIndex = cv.candPosIds.indexOf(posId);
        cv.candPosIds.splice(posIndex, 1);
        posIndex = cv.cvPosIds.indexOf(posId);
        cv.cvPosIds.splice(posIndex, 1);
      }

      candPosIds = [...cv.candPosIds];
      cvPosIds = [...cv.cvPosIds];

      this.updateListCvsPosIds(
        this.cvsList,
        cv.candidateId,
        cv.cvId,
        candPosIds,
        cvPosIds
      );
      this.updateListCvsPosIds(
        this.duplicatesCvsList,
        cv.candidateId,
        cv.cvId,
        candPosIds,
        cvPosIds
      );

      runInAction(() => {
        this.cvDisplayed = cv;
      });

      const res = await this.cvsApi.AttachePosCv(
        cv.candidateId,
        cv.cvId,
        posId,
        candPosIds,
        cvPosIds,
        checked
      );
    }
  }

  async updateListCvsPosIds(
    list: ICv[],
    candId: number,
    cvId: number,
    candPosIds: number[],
    cvPosIds: number[]
  ) {
    const candCvs = list.filter((x) => x.candidateId === candId);

    candCvs.forEach((cv) => {
      cv.candPosIds = [...candPosIds];

      if (cv.cvId === cvId) {
        cv.cvPosIds = [...cvPosIds];
      }
    });
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
