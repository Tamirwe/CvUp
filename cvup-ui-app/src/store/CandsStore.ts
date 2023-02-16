import { makeAutoObservable, runInAction } from "mobx";
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
  candDisplay?: ICand;
  candListDisplay: DisplayedCvsListEnum = DisplayedCvsListEnum.None;
  tabDisplayCandsList: string = "candList";

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
  }

  reset() {
    this.candsList = [];
    this.candSelected = undefined;
  }

  set cvReviewDialogOpen(val) {
    this.isCvReviewDialogOpen = val;
  }

  get cvReviewDialogOpen() {
    return this.isCvReviewDialogOpen;
  }

  set currentTabCandsList(val) {
    this.tabDisplayCandsList = val;
  }

  get currentTabCandsList() {
    return this.tabDisplayCandsList;
  }

  async displayCvMain(cand: ICand) {
    runInAction(() => {
      this.candSelected = cand;
      this.candDisplay = this.candSelected;
      this.candListDisplay = DisplayedCvsListEnum.SearchCvsList;
      this.getPdf();
    });
  }

  async displayCvDuplicate(cand: ICand) {
    runInAction(() => {
      this.candDupSelected = cand;
      this.candDisplay = this.candDupSelected;
      this.candListDisplay = DisplayedCvsListEnum.DuplicateCvsList;
      this.getPdf();
    });
  }

  async displayCvPosition(cand: ICand) {
    runInAction(() => {
      this.candPosSelected = cand;
      this.candDisplay = this.candPosSelected;
      this.candListDisplay = DisplayedCvsListEnum.PositionCvsList;
      this.getPdf();
    });
  }

  async getPdf() {
    this.pdfUrl = `${this.appSettings.appServerUrl}DD?id=${this.candDisplay?.keyId}`;
  }

  async saveCvReview(reviewText: any, reviewHtml: any) {
    if (this.candSelected?.candidateId) {
      const cvReview: ICvReview = {
        candidateId: this.candSelected?.candidateId,
        reviewText,
        reviewHtml,
      };
      await this.cvsApi.saveCvReview(cvReview);
    }

    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay.review = reviewHtml;
        let cand = this.candsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );
        if (cand) {
          cand.review = reviewHtml;
        }

        const candDup = this.candDupCvsList.filter(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        candDup.forEach((cand) => {
          cand.review = reviewHtml;
        });

        cand = this.posCandsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.review = reviewHtml;
        }
      }
    });
  }

  async searchCands(value: string) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(value);
    runInAction(() => {
      this.candsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getCandsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCandsList();
    runInAction(() => {
      this.candsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getDuplicatesCvsList(cand: ICand) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getDuplicatesCvsList(
      cand.cvId,
      cand.candidateId
    );
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
    if (this.candDisplay) {
      await this.cvsApi.attachPosCandCv(
        this.candDisplay.candidateId,
        this.candDisplay.cvId,
        positionId,
        this.candDisplay.keyId
      );
    }

    if (this.candDisplay) {
      this.candDisplay.candPosIds.push(positionId);
      this.candDisplay.cvPosIds.push(positionId);

      const cand = this.candsList.find(
        (x) => x.candidateId === this.candDisplay?.candidateId
      );

      if (cand) {
        this.updateCandPosArrays(cand, this.candDisplay);
      }

      const candDup = this.candDupCvsList.filter(
        (x) => x.candidateId === this.candDisplay?.candidateId
      );

      candDup.forEach((cand) => {
        this.updateCandPosArrays(cand, this.candDisplay!);
      });

      if (this.rootStore.positionsStore.posSelected?.id === positionId) {
        this.getPositionCands(positionId);
      }
    }
  }

  async detachPosCandidate(posCand: ICand, index: number) {
    const positionId = this.rootStore.positionsStore.posSelected?.id;

    if (positionId) {
      await this.cvsApi.detachPosCand(
        posCand.candidateId,
        posCand.cvId,
        positionId
      );

      this.unPushNumArr(positionId, posCand.candPosIds);
      this.unPushNumArr(positionId, posCand.cvPosIds);

      if (this.candDisplay?.candidateId === posCand.candidateId) {
        this.updateCandPosArrays(this.candDisplay, posCand);
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
