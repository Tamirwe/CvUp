import { makeAutoObservable, runInAction, toJS } from "mobx";
import { DisplayedCvsListEnum } from "../models/GeneralEnums";
import { IAppSettings, ICand, ICvReview } from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";

export class CandsStore {
  private cvsApi;
  private isCvReviewDialogOpen: boolean = false;
  candsList: ICand[] = [];
  candDupCvsList: ICand[] = [];
  posCandsList: ICand[] = [];
  pdfUrl: string = "";
  candSelected?: ICand;
  candDupSelected?: ICand;
  candPosSelected?: ICand;
  candDisplaying?: ICand;
  cvDisplayedList: DisplayedCvsListEnum = DisplayedCvsListEnum.None;

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
  }

  reset() {
    this.candsList = [];
    this.candSelected = undefined;
  }

  set openCvReviewDialogOpen(val) {
    this.isCvReviewDialogOpen = val;
  }

  get openCvReviewDialogOpen() {
    return this.isCvReviewDialogOpen;
  }

  async displayCvMain(cand: ICand) {
    runInAction(() => {
      this.candSelected = cand;
      this.candDisplaying = this.candSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.SearchCvsList;
      this.getPdf();
    });
  }

  async displayCvDuplicate(cand: ICand) {
    runInAction(() => {
      this.candDupSelected = cand;
      this.candDisplaying = this.candDupSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.DuplicateCvsList;
      this.getPdf();
    });
  }

  async displayCvPosition(cand: ICand) {
    runInAction(() => {
      this.candPosSelected = cand;
      this.candDisplaying = this.candPosSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.PositionCvsList;
      this.getPdf();
    });
  }

  async getPdf() {
    this.pdfUrl = `${this.appSettings.appServerUrl}DD?id=${this.candDisplaying?.keyId}`;
  }

  async saveCvReview(reviewText: any, reviewHtml: any) {
    // const cvReview:ICvReview = {  candidateId:};
    // const res = await this.cvsApi.saveCvReview(cvReview);
  }

  async getCandsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCandsList();
    runInAction(() => {
      this.candsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getDuplicatesCvsList(cv: ICand) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getDuplicatesCvsList(cv.cvId, cv.candidateId);
    runInAction(() => {
      this.candDupCvsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionCands(positionId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.GetPosCandsList(positionId);
    runInAction(() => {
      this.posCandsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async attachPosCandCv(positionId: number) {
    if (this.candDisplaying) {
      const res = await this.cvsApi.attachPosCandCv(
        this.candDisplaying.candidateId,
        this.candDisplaying.cvId,
        positionId,
        this.candDisplaying.keyId
      );
    }

    if (this.candDisplaying) {
      this.candDisplaying.candPosIds.push(positionId);
      this.candDisplaying.cvPosIds.push(positionId);

      const cand = this.candsList.find(
        (x) => x.candidateId === this.candDisplaying?.candidateId
      );

      if (cand) {
        this.updateCandPosArrays(cand, this.candDisplaying);
      }

      const candDup = this.candDupCvsList.filter(
        (x) => x.candidateId === this.candDisplaying?.candidateId
      );

      candDup.forEach((cand) => {
        this.updateCandPosArrays(cand, this.candDisplaying!);
      });

      this.rootStore.positionsStore.setPosSelected(positionId);
      this.getPositionCands(positionId);
    }
  }

  async detachPosCandidate(positionId: number, posCand: ICand, index: number) {
    const res = await this.cvsApi.detachPosCand(
      posCand.candidateId,
      posCand.cvId,
      positionId
    );

    this.unPushNumArr(positionId, posCand.candPosIds);
    this.unPushNumArr(positionId, posCand.cvPosIds);

    if (this.candDisplaying?.candidateId === posCand.candidateId) {
      this.updateCandPosArrays(this.candDisplaying, posCand);
    }

    const cand = this.candsList.find(
      (x) => x.candidateId === posCand.candidateId
    );

    if (cand) {
      this.updateCandPosArrays(cand, posCand);
    }

    const candDup = this.candDupCvsList.filter(
      (x) => x.candidateId === posCand.candidateId
    );

    candDup.forEach((cand) => {
      this.updateCandPosArrays(cand, posCand);
    });

    this.posCandsList.splice(index, 1);
  }

  updateCandPosArrays(candTarget: ICand, candSource: ICand) {
    candTarget.candPosIds = [...candSource.candPosIds];

    if (candTarget.cvId === candSource.cvId) {
      candTarget.cvPosIds = [...candSource.cvPosIds];
    }
  }

  unPushNumArr(id: number, numArr?: number[]) {
    if (numArr) {
      let index = numArr.indexOf(id);

      if (index > -1) {
        numArr.splice(index, 1);
      }
    }
  }
}
