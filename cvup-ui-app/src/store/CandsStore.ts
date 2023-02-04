import { makeAutoObservable, runInAction, toJS } from "mobx";
import { DisplayedCvsListEnum } from "../models/GeneralEnums";
import { IAppSettings, ICv, ICvReview } from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";

export class CandsStore {
  private cvsApi;
  private isCvReviewDialogOpen: boolean = false;
  cvsList: ICv[] = [];
  duplicatesCvsList: ICv[] = [];
  posCvsList: ICv[] = [];
  pdfUrl: string = "";
  cvSelected?: ICv;
  cvDuplicateSelected?: ICv;
  cvPositionSelected?: ICv;
  cvDisplayed?: ICv;
  cvDisplayedList: DisplayedCvsListEnum = DisplayedCvsListEnum.None;

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
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

  async displayCvMain(cv: ICv) {
    runInAction(() => {
      this.cvSelected = cv;
      this.cvDisplayed = this.cvSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.SearchCvsList;
      this.getPdf();
    });
  }

  async displayCvDuplicate(cv: ICv) {
    runInAction(() => {
      this.cvDuplicateSelected = cv;
      this.cvDisplayed = this.cvDuplicateSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.DuplicateCvsList;
      this.getPdf();
    });
  }

  async displayCvPosition(cv: ICv) {
    runInAction(() => {
      this.cvPositionSelected = cv;
      this.cvDisplayed = this.cvPositionSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.PositionCvsList;
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

  async getDuplicatesCvsList(cv: ICv) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getDuplicatesCvsList(cv.cvId, cv.candidateId);
    runInAction(() => {
      this.duplicatesCvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionCvs(positionId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getPosCvsList(positionId);
    runInAction(() => {
      this.posCvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async attachPosCandCv(positionId: number) {
    if (this.cvDisplayed) {
      const res = await this.cvsApi.attachPosCandCv(
        this.cvDisplayed.candidateId,
        this.cvDisplayed.cvId,
        positionId,
        this.cvDisplayed.keyId
      );
    }
    this.updateCvsLists(positionId);
  }

  async detachPosCandidate(
    positionId: number,
    cvId: number,
    candidateId: number
  ) {
    const res = await this.cvsApi.detachPosCandidate(
      candidateId,
      cvId,
      positionId
    );
  }

  updateCvsLists(positionId: number) {
    // this.posCvsList.find(x=>x.)
  }

  // const cv = toJS(this.cvDisplayed);
  // let candPosIds: number[] = [];
  // let cvPosIds: number[] = [];

  // if (cv && cv.candPosIds) {
  //   if (checked) {
  //     cv.candPosIds.push(positionId);
  //     cv.cvPosIds.push(positionId);
  //   } else {
  //     let posIndex = cv.candPosIds.indexOf(positionId);
  //     cv.candPosIds.splice(posIndex, 1);
  //     posIndex = cv.cvPosIds.indexOf(positionId);
  //     cv.cvPosIds.splice(posIndex, 1);
  //   }

  //   candPosIds = [...cv.candPosIds];
  //   cvPosIds = [...cv.cvPosIds];

  //   this.updateListCvsPosIds(
  //     this.cvsList,
  //     cv.candidateId,
  //     cv.cvId,
  //     candPosIds,
  //     cvPosIds
  //   );
  //   this.updateListCvsPosIds(
  //     this.duplicatesCvsList,
  //     cv.candidateId,
  //     cv.cvId,
  //     candPosIds,
  //     cvPosIds
  //   );

  //   runInAction(() => {
  //     this.cvDisplayed = cv;
  //   });
  // }
  // }

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
