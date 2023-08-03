import { makeAutoObservable, runInAction } from "mobx";
import {
  CandsSourceEnum,
  CvDisplayedListEnum,
  TabsCandsEnum,
} from "../models/GeneralEnums";
import {
  IAppSettings,
  ICand,
  ICandCv,
  ICandPosStage,
  IPosStagesType,
  IEmailTemplate,
  ISendEmail,
  ISearchModel,
  ICandsReport,
} from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";

export class CandsStore {
  private cvsApi;
  private candIdDuplicateCvs: number = 0;
  allCandsList: ICand[] = [];
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
  posStages?: IPosStagesType[];
  emailTemplates?: IEmailTemplate[];
  candsReportData?: ICandsReport[];

  constructor(private rootStore: RootStore, private appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.cvsApi = new CandsApi(appSettings);
  }

  get duplicateCvsCandId() {
    return this.candIdDuplicateCvs;
  }

  set duplicateCvsCandId(val) {
    this.candIdDuplicateCvs = val;
  }

  reset() {
    this.allCandsList = [];
    this.candAllSelected = undefined;
  }

  set currentTabCandsLists(val) {
    this.tabDisplayCandsLists = val;
  }

  get currentTabCandsLists() {
    return this.tabDisplayCandsLists;
  }

  async displayCv(cand: ICand, candsSource: CandsSourceEnum) {
    runInAction(() => {
      // this.candAllSelected = cand;
      this.candDisplay = cand;
      this.rootStore.positionsStore.setRelatedPositionToCandDisplay(
        candsSource
      );

      this.getPdf(cand.keyId);

      if (!cand.isSeen) {
        this.cvsApi.updateIsSeen(cand.cvId);
        cand.isSeen = true;

        //not must but any way
        const updatedCand = Object.assign({}, cand);
        this.updateCandAllList(updatedCand);
        this.updatePosCandList(updatedCand);
        this.updateFolderCandList(updatedCand);
      }
    });
  }

  async displayCvDuplicate(candCv: ICandCv, listType: CvDisplayedListEnum) {
    runInAction(() => {
      this.candDupSelected = candCv;

      if (this.candDisplay) {
        this.candDisplay!.cvId = candCv.cvId;
        this.candDisplay!.keyId = candCv.keyId;
      }

      switch (listType) {
        case CvDisplayedListEnum.CandsList:
          this.candAllSelected = this.allCandsList.find(
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

  async getPdf(keyId: string) {
    this.pdfUrl = `${this.appSettings.apiUrl}DD?id=${keyId}`;
  }

  async saveCandReview(review: string) {
    this.rootStore.generalStore.backdrop = true;

    if (this.candDisplay) {
      const res = await this.cvsApi.saveCandReview(
        review,
        this.candDisplay?.candidateId
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateCandAllList(res.data);
          this.updatePosCandList(res.data);
          this.updateFolderCandList(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async saveCandDetails(
    firstName: string,
    lastName: string,
    email: string,
    phone: string
  ) {
    this.rootStore.generalStore.backdrop = true;

    if (this.candDisplay) {
      const res = await this.cvsApi.saveCandDetails(
        this.candDisplay?.candidateId,
        firstName,
        lastName,
        email,
        phone
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateCandAllList(res.data);
          this.updatePosCandList(res.data);
          this.updateFolderCandList(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async searchAllCands(searchVals: ISearchModel) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(searchVals, 0, 0);
    runInAction(() => {
      this.allCandsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async searchPositionCands(searchVals: ISearchModel) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(
      searchVals,
      this.rootStore.positionsStore.selectedPosition?.id
    );
    runInAction(() => {
      this.posCandsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async searchFolderCands(searchVals: ISearchModel) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(
      searchVals,
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
      this.allCandsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getDuplicatesCvsList(cand: ICand) {
    this.rootStore.generalStore.backdrop = true;

    if (this.duplicateCvsCandId !== cand.candidateId) {
      const res = await this.cvsApi.getDuplicatesCvsList(
        cand.cvId,
        cand.candidateId
      );

      runInAction(() => {
        this.duplicateCvsCandId = cand.candidateId;
        this.candDupCvsList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionCandsList(posId: number) {
    this.rootStore.generalStore.backdrop = true;

    runInAction(async () => {
      this.posCandsList = [];

      const res = await this.cvsApi.getPosCandsList(posId);
      this.posCandsList = res.data;
    });

    this.rootStore.generalStore.backdrop = false;
  }

  async setDisplayCandOntopPCList() {
    runInAction(async () => {
      if (this.candDisplay?.candidateId) {
        const candsList = [...this.posCandsList];

        const objIndex = candsList.findIndex(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (objIndex > -1) {
          const candSelected = candsList.splice(objIndex, 1);
          candsList.splice(0, 0, candSelected[0]);
          this.posCandsList = candsList;
        }
      }
    });
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
      const res = await this.cvsApi.attachPosCandCv(
        this.candDisplay.candidateId,
        this.candDisplay.cvId,
        positionId,
        this.candDisplay.keyId
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateCandAllList(res.data);
          this.updateFolderCandList(res.data);
          this.updatePosCandList(res.data);
        }
      });
    }
  }

  async detachPosCand(detachCand: ICand, positionId: number, index: number) {
    if (positionId) {
      const res = await this.cvsApi.detachPosCand(
        detachCand.candidateId,
        detachCand.cvId,
        positionId
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateCandAllList(res.data);
          this.updatePosCandList(res.data);
          this.updateFolderCandList(res.data);

          if (
            this.rootStore.positionsStore.selectedPosition?.id === positionId
          ) {
            this.removePosCandList(res.data);
          }
        }
      });
    }
  }

  updateCandAllList(cand: ICand) {
    const candsList = [...this.allCandsList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList[index] = { ...cand };
      this.allCandsList = candsList;
    }
  }

  updatePosCandList(cand: ICand) {
    const updatedCand = { ...cand };
    const candsList = [...this.posCandsList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      const listCand = candsList[index];
      updatedCand.cvId = listCand.cvId;
      updatedCand.emailSubject = listCand.emailSubject;
      updatedCand.keyId = listCand.keyId;
      updatedCand.isSeen = listCand.isSeen;

      candsList[index] = updatedCand;
      this.posCandsList = candsList;
    }
  }

  updateFolderCandList(cand: ICand) {
    const candsList = [...this.folderCandsList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList[index] = { ...cand };
      this.posCandsList = candsList;
    }
  }

  removePosCandList(cand: ICand) {
    const candsList = [...this.posCandsList];
    const index = this.posCandsList.findIndex(
      (x) => x.candidateId === cand.candidateId
    );
    candsList.splice(index, 1);
    this.posCandsList = candsList;
  }

  allCandsListCand(candidateId: number) {
    const varToPreventTsError: any = {};

    const cand = this.allCandsList.find(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );
    return cand ? cand : varToPreventTsError;
  }

  candsPosListCand(candidateId: number) {
    const varToPreventTsError: any = {};

    const cand = this.posCandsList.find(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );
    return cand ? cand : varToPreventTsError;
  }

  candsFolderListCand(candidateId: number) {
    const varToPreventTsError: any = {};

    const cand = this.folderCandsList.find(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );
    return cand ? cand : varToPreventTsError;
  }

  async updateCandFoldersIds(candFoldersIdsArr: number[]) {
    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay.candFoldersIds = candFoldersIdsArr;

        const candId = this.candDisplay?.candidateId;
        this.allCandsListCand(candId).candFoldersIds = candFoldersIdsArr;
        this.candsPosListCand(candId).candFoldersIds = candFoldersIdsArr;
        this.candsFolderListCand(candId).candFoldersIds = candFoldersIdsArr;
      }
    });
  }

  async updateCandPositionStatus(stageType: string | ICandPosStage) {
    this.rootStore.generalStore.backdrop = true;
    const candDisplayPosition =
      this.rootStore.positionsStore.candDisplayPosition;

    if (this.candDisplay?.candidateId && candDisplayPosition?.id) {
      const res = await this.cvsApi.updateCandPositionStatus(
        stageType.toString(),
        this.candDisplay?.candidateId,
        candDisplayPosition?.id
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateCandAllList(res.data);
          this.updatePosCandList(res.data);
          this.updateFolderCandList(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  objArrRemoveItem(id: number, objArr: any, col: string) {
    let index = objArr.findIndex((x: any) => x[col] === id);

    if (index > -1) {
      objArr.splice(index, 1);
    }
  }

  async getCandPosStages() {
    const res = await this.cvsApi.getCandPosStages();
    runInAction(() => {
      this.posStages = res.data;
    });
  }

  findStageName(stageType: string) {
    if (this.posStages) {
      return this.posStages.find((x) => x.stageType === stageType)?.name;
    }

    return "";
  }

  findStageColor(stageType: string) {
    if (this.posStages) {
      return (
        this.posStages.find((x) => x.stageType === stageType)?.color || "black"
      );
    }

    return "";
  }

  sortPosStage(posStages: ICandPosStage[]) {
    return posStages
      .slice()
      .sort((a, b) => (a.d < b.d ? 1 : b.d < a.d ? -1 : 0));
  }

  async getEmailTemplates() {
    const res = await this.cvsApi.getEmailTemplates();
    runInAction(() => {
      this.emailTemplates = res.data;
    });
  }

  async addUpdateEmailTemplate(emailTemplate: IEmailTemplate) {
    const res = await this.cvsApi.addUpdateEmailTemplate(emailTemplate);
    return res;
  }

  async deleteEmailTemplate(id: number) {
    const res = await this.cvsApi.deleteEmailTemplate(id);
    return res;
  }

  async sendEmailToCandidate(
    emailData: ISendEmail,
    emailTemplate: IEmailTemplate
  ) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.sendEmail(emailData);

    if (res.isSuccess && emailTemplate && emailTemplate.stageToUpdate) {
      this.updateCandPositionStatus(emailTemplate.stageToUpdate);
    }

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async sendEmailTocontact(emailData: ISendEmail) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.sendEmail(emailData);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async getCandsReport(stageType: string) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.getCandsReport(stageType);

    runInAction(() => {
      this.candsReportData = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }
}
