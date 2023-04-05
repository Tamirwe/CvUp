import { makeAutoObservable, runInAction } from "mobx";
import { CvDisplayedListEnum, TabsCandsEnum } from "../models/GeneralEnums";
import {
  IAppSettings,
  ICand,
  ICandCv,
  ICvReview,
} from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";

export class CandsStore {
  private cvsApi;
  candsAllList: ICand[] = [];
  candDupCvsList: ICandCv[] = [];
  posCandsList: ICand[] = [];
  folderCandsList: ICand[] = [];
  pdfUrl: string = "";
  candAllSelected?: ICand;
  candDupSelected?: ICandCv;
  candPosSelected?: ICand;
  candFolderSelected?: ICand;
  candDisplay?: ICand;
  private tabDisplayCandsLists: TabsCandsEnum = TabsCandsEnum.AllCands;

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
  }

  reset() {
    this.candsAllList = [];
    this.candAllSelected = undefined;
  }

  set currentTabCandsLists(val) {
    this.tabDisplayCandsLists = val;
  }

  get currentTabCandsLists() {
    return this.tabDisplayCandsLists;
  }

  async displayCvMain(cand: ICand) {
    runInAction(() => {
      this.candAllSelected = cand;
      this.candDisplay = this.candAllSelected;
      this.getPdf(cand.keyId);
    });
  }

  async displayCvDuplicate(candCv: ICandCv, listType: CvDisplayedListEnum) {
    runInAction(() => {
      this.candDupSelected = candCv;
      // this.candDisplay = this.candDupSelected;

      switch (listType) {
        case CvDisplayedListEnum.CandsList:
          this.candAllSelected = this.candsAllList.find(
            (x) => x.candidateId === candCv.candidateId
          );
          break;
        case CvDisplayedListEnum.PositionCandsList:
          this.candPosSelected = this.posCandsList.find(
            (x) => x.candidateId === candCv.candidateId
          );
          break;
        case CvDisplayedListEnum.FolderCandsList:
          this.candFolderSelected = this.folderCandsList.find(
            (x) => x.candidateId === candCv.candidateId
          );
          break;
        default:
          break;
      }
      this.getPdf(candCv.keyId);
    });
  }

  async displayCvPosition(cand: ICand) {
    runInAction(() => {
      this.candPosSelected = cand;
      this.candDisplay = this.candPosSelected;
      this.getPdf(cand.keyId);
    });
  }

  async getPdf(keyId: string) {
    this.pdfUrl = `${this.appSettings.appServerUrl}DD?id=${keyId}`;
  }

  async saveCvReview(reviewText: any, reviewHtml: any) {
    if (this.candAllSelected?.candidateId) {
      const cvReview: ICvReview = {
        candidateId: this.candAllSelected?.candidateId,
        reviewText,
        reviewHtml,
      };
      await this.cvsApi.saveCvReview(cvReview);
    }

    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay.review = reviewHtml;

        let cand = this.candsAllList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.review = reviewHtml;
        }

        cand = this.posCandsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.review = reviewHtml;
        }

        cand = this.folderCandsList.find(
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
      this.candsAllList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getCandsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getCandsList();
    runInAction(() => {
      this.candsAllList = res.data;
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

  async getFolderCandsList(folderId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getFolderCandsList(folderId);
    runInAction(() => {
      this.folderCandsList = res.data;
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

    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay.candPosIds.push(positionId);
        this.candDisplay.cvPosIds.push(positionId);

        let cand = this.candsAllList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          this.updateCandPosArrays(cand, this.candDisplay);
        }

        cand = this.folderCandsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          this.updateCandPosArrays(cand, this.candDisplay);
        }

        if (this.rootStore.positionsStore.selectedPosition?.id === positionId) {
          this.getPositionCands(positionId);
        }
      }
    });
  }

  async detachPosCand(detachCand: ICand, index: number) {
    const positionId = this.rootStore.positionsStore.selectedPosition?.id;

    if (positionId) {
      await this.cvsApi.detachPosCand(
        detachCand.candidateId,
        detachCand.cvId,
        positionId
      );

      this.unPushNumArr(positionId, detachCand.candPosIds);
      this.unPushNumArr(positionId, detachCand.cvPosIds);

      if (this.candDisplay?.candidateId === detachCand.candidateId) {
        this.updateCandPosArrays(this.candDisplay, detachCand);
      }

      let cand = this.candsAllList.find(
        (x) => x.candidateId === detachCand.candidateId
      );

      if (cand) {
        this.updateCandPosArrays(cand, detachCand);
      }

      cand = this.folderCandsList.find(
        (x) => x.candidateId === detachCand.candidateId
      );

      if (cand) {
        this.updateCandPosArrays(cand, detachCand);
      }

      this.posCandsList.splice(index, 1);
    }
  }

  async detachFolderCand(detachCand: ICand, index: number) {
    await this.cvsApi.detachFolderCand(detachCand.folderCandId);

    this.folderCandsList.splice(index, 1);
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
