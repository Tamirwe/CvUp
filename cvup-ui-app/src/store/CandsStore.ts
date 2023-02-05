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
  candDisplayed?: ICand;
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
      this.candDisplayed = this.candSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.SearchCvsList;
      this.getPdf();
    });
  }

  async displayCvDuplicate(cand: ICand) {
    runInAction(() => {
      this.candDupSelected = cand;
      this.candDisplayed = this.candDupSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.DuplicateCvsList;
      this.getPdf();
    });
  }

  async displayCvPosition(cand: ICand) {
    runInAction(() => {
      this.candPosSelected = cand;
      this.candDisplayed = this.candPosSelected;
      this.cvDisplayedList = DisplayedCvsListEnum.PositionCvsList;
      this.getPdf();
    });
  }

  async getPdf() {
    this.pdfUrl = `${this.appSettings.appServerUrl}DD?id=${this.candDisplayed?.keyId}`;
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
    if (this.candDisplayed) {
      const res = await this.cvsApi.attachPosCandCv(
        this.candDisplayed.candidateId,
        this.candDisplayed.cvId,
        positionId,
        this.candDisplayed.keyId
      );
    }

    await this.updateCandsPositionArrays(
      positionId,
      true,
      this.candDisplayed?.candidateId,
      this.candDisplayed?.cvId,
      this.candDisplayed?.keyId
    );
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

    await this.updateCandsPositionArrays(positionId, false, candidateId, cvId);
  }

  async updateCandsPositionArrays(
    positionId: number,
    isAttach: boolean,
    candidateId?: number,
    cvId?: number,
    keyId?: string
  ) {
    if (candidateId && cvId) {
      this.updateCandsListPosArrays(positionId, isAttach, candidateId, cvId);
      this.updateCandDupCvsListPosArrays(
        positionId,
        isAttach,
        candidateId,
        cvId
      );
      await this.updatePosCandsListPosArrays(
        positionId,
        isAttach,
        candidateId,
        cvId,
        keyId
      );
    }
  }

  updateCandsListPosArrays(
    positionId: number,
    isAttach: boolean,
    candidateId: number,
    cvId: number
  ) {
    const cand = this.posCandsList.find((x) => x.candidateId === candidateId);

    if (cand) {
      if (isAttach) {
        cand.candPosIds.push(positionId);
      } else {
        this.unPushNumArr(positionId, cand?.candPosIds);
      }

      if (cand.cvId === cvId) {
        if (isAttach) {
          cand.cvPosIds.push(positionId);
        } else {
          this.unPushNumArr(positionId, cand?.cvPosIds);
        }
      }
    }
  }

  updateCandDupCvsListPosArrays(
    positionId: number,
    isAttach: boolean,
    candidateId: number,
    cvId: number
  ) {
    this.posCandsList.forEach((candDupCv) => {
      if (candDupCv?.candidateId === candidateId) {
        if (isAttach) {
          candDupCv?.candPosIds.push(positionId);
        } else {
          this.unPushNumArr(positionId, candDupCv?.candPosIds);
        }

        if (candDupCv.cvId === cvId) {
          if (isAttach) {
            candDupCv.cvPosIds.push(positionId);
          } else {
            this.unPushNumArr(positionId, candDupCv?.cvPosIds);
          }
        }
      }
    });
  }

  async updatePosCandsListPosArrays(
    positionId: number,
    isAttach: boolean,
    candidateId: number,
    cvId: number,
    keyId?: string
  ) {
    if (isAttach) {
      if (this.rootStore.positionsStore.posSelected?.id === positionId) {
        const cand = this.posCandsList.find(
          (x) => x.candidateId === candidateId
        );

        if (cand && keyId) {
          cand.candCvs.push({ cvId, keyId, isSentByEmail: false });
        }
      } else {
        await this.getPositionCands(positionId);
      }
    } else {
      const index = this.posCandsList.findIndex(
        (x) => x.candidateId === candidateId
      );

      if (index > -1) {
        this.posCandsList.splice(index, 1);
      }
    }
  }

  unPushNumArr(id: number, numArr?: number[]) {
    if (numArr) {
      let index = numArr.indexOf(id);

      if (index && index > -1) {
        numArr.splice(index, 1);
      }
    }
  }

  async updateListCvsPosIds(
    list: ICand[],
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
}
