import { makeAutoObservable, runInAction } from "mobx";
import { CvDisplayedListEnum, TabsCandsEnum } from "../models/GeneralEnums";
import {
  IAppSettings,
  ICand,
  ICandCv,
  ICvReview,
  ICompanyStagesTypes,
  IPosStages,
} from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";
import { format } from "date-fns";

export class CandsStore {
  private cvsApi;
  private cvIdDuplicatesList: number = 0;
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
  stagesTypes?: ICompanyStagesTypes[];

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
  }

  get duplicateCvId() {
    return this.cvIdDuplicatesList;
  }

  set duplicateCvId(val) {
    this.cvIdDuplicatesList = val;
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
    this.pdfUrl = `${this.appSettings.apiUrl}DD?id=${keyId}`;
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

  async saveCandReview(review: string) {
    this.rootStore.generalStore.backdrop = true;

    if (this.candDisplay) {
      const data = await this.cvsApi.saveCandReview(
        review,
        this.candDisplay?.candidateId
      );

      runInAction(() => {
        if (this.candDisplay) {
          this.candDisplay.review = review;

          let cand = this.candsAllList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.review = review;
          }

          cand = this.posCandsList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.review = review;
          }

          cand = this.folderCandsList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.review = review;
          }
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async searchAllCands(value: string) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(value, 0, 0);
    runInAction(() => {
      this.candsAllList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async searchPositionCands(value: string) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(
      value,
      this.rootStore.positionsStore.selectedPosition?.id
    );
    runInAction(() => {
      this.posCandsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async searchFolderCands(value: string) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(
      value,
      0,
      this.rootStore.foldersStore.selectedFolder?.id
    );
    runInAction(() => {
      this.folderCandsList = res.data;
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

  async getPositionCands() {
    this.posCandsList = [];

    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.GetPosCandsList(
      this.rootStore.positionsStore.selectedPosition?.id!
    );

    const candsList = [...res.data];

    if (this.rootStore.positionsStore.selectedPositionCandId) {
      const objIndex = candsList.findIndex(
        (x) =>
          x.candidateId === this.rootStore.positionsStore.selectedPositionCandId
      );

      if (objIndex > -1) {
        const candSelected = candsList.splice(objIndex, 1);
        candsList.splice(0, 0, candSelected[0]);
      }
    }

    // runInAction(() => {
    this.posCandsList = [...candsList];
    // });

    this.rootStore.generalStore.backdrop = false;
  }

  async getFolderCandsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.getFolderCandsList(
      this.rootStore.foldersStore.selectedFolder?.id!
    );
    runInAction(() => {
      this.tabDisplayCandsLists = TabsCandsEnum.FolderCands;
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

    // runInAction(() => {
    if (this.candDisplay) {
      this.candDisplay.candPosIds.push(positionId);
      this.candDisplay.cvPosIds.push(positionId);
      this.candDisplay.posStages?.push({
        d: format(new Date(), "yyyy-MM-dd"),
        id: positionId,
        t: "attached_to_position",
      });

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
        this.getPositionCands();
      }
    }
    // });
  }

  async detachPosCand(detachCand: ICand, positionId: number, index: number) {
    // const positionId = this.rootStore.positionsStore.selectedPosition?.id;

    if (positionId) {
      await this.cvsApi.detachPosCand(
        detachCand.candidateId,
        detachCand.cvId,
        positionId
      );

      this.numArrRemoveItem(positionId, detachCand.candPosIds);
      this.numArrRemoveItem(positionId, detachCand.cvPosIds);
      this.objArrRemoveItem(positionId, detachCand.posStages, "id");

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

  numArrRemoveItem(id: number, numArr?: number[]) {
    if (numArr) {
      let index = numArr.indexOf(id);

      if (index > -1) {
        numArr.splice(index, 1);
      }
    }
  }

  objArrRemoveItem(id: number, objArr: any, col: string) {
    let index = objArr.findIndex((x: any) => x[col] === id);

    if (index > -1) {
      objArr.splice(index, 1);
    }
  }

  async getCompanyStagesTypes() {
    const res = await this.cvsApi.getCompanyStagesTypes();
    runInAction(() => {
      this.stagesTypes = res.data;
    });
  }

  findStageName(stageType: string) {
    if (this.stagesTypes) {
      return this.stagesTypes.find((x) => x.stageType === stageType)?.name;
    }

    return "";
  }

  findStageColor(stageType: string) {
    if (this.stagesTypes) {
      return (
        this.stagesTypes.find((x) => x.stageType === stageType)?.color ||
        "black"
      );
    }

    return "";
  }

  sortPosStage(posStages: IPosStages[]) {
    return posStages
      .slice()
      .sort((a, b) => (a.d < b.d ? 1 : b.d < a.d ? -1 : 0));
  }
}
