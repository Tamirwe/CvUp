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
  ICompanyStagesTypes,
  IPosStages,
  IEmailTemplate,
  IEmailsAddress,
  IPosition,
  IAttachCv,
  ISendEmail,
  ISearchModel,
} from "../models/GeneralModels";
import CandsApi from "./api/CandsApi";
import { RootStore } from "./RootStore";
import { format } from "date-fns";
import { numArrRemoveItem } from "../utils/GeneralUtils";

export class CandsStore {
  private cvsApi;
  private candIdDuplicateCvs: number = 0;
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
  emailTemplates?: IEmailTemplate[];

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
    this.candsAllList = [];
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
      cand.position = undefined;

      if (candsSource === CandsSourceEnum.Position) {
        const selectedPosition = this.rootStore.positionsStore.selectedPosition;

        if (this.candDisplay) {
          this.candDisplay.position = selectedPosition;
        }
      }

      this.getPdf(cand.keyId);
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

  async getPdf(keyId: string) {
    this.pdfUrl = `${this.appSettings.apiUrl}DD?id=${keyId}`;
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

  async saveCandDetails(
    firstName: string,
    lastName: string,
    email: string,
    phone: string
  ) {
    this.rootStore.generalStore.backdrop = true;

    if (this.candDisplay) {
      const data = await this.cvsApi.saveCandDetails(
        this.candDisplay?.candidateId,
        firstName,
        lastName,
        email,
        phone
      );

      runInAction(() => {
        if (this.candDisplay) {
          this.candDisplay.firstName = firstName;
          this.candDisplay.lastName = lastName;
          this.candDisplay.email = email;
          this.candDisplay.phone = phone;

          let cand = this.candsAllList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.firstName = firstName;
            cand.lastName = lastName;
            cand.email = email;
            cand.phone = phone;
          }

          cand = this.posCandsList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.firstName = firstName;
            cand.lastName = lastName;
            cand.email = email;
            cand.phone = phone;
          }

          cand = this.folderCandsList.find(
            (x) => x.candidateId === this.candDisplay?.candidateId
          );

          if (cand) {
            cand.firstName = firstName;
            cand.lastName = lastName;
            cand.email = email;
            cand.phone = phone;
          }
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async searchAllCands(searchVals: ISearchModel) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.cvsApi.searchCands(searchVals, 0, 0);
    runInAction(() => {
      this.candsAllList = res.data;
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
      this.candsAllList = res.data;
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

  async getPositionCands() {
    this.posCandsList = [];

    this.rootStore.generalStore.backdrop = true;
    const posId = this.rootStore.positionsStore.selectedPosition?.id!;

    const res = await this.cvsApi.getPosCandsList(posId);

    const candsList = [...res.data];

    const objIndex = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (objIndex > -1) {
      const candSelected = candsList.splice(objIndex, 1);
      candsList.splice(0, 0, candSelected[0]);
    }

    runInAction(() => {
      this.posCandsList = [...candsList];
    });

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
          this.updatePosCandList(res.data);
          this.updateFolderCandList(res.data);

          if (
            this.rootStore.positionsStore.selectedPosition?.id === positionId
          ) {
            this.addPosCandList(res.data);
          }
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
    const candsList = [...this.candsAllList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList[index] = { ...cand };
      this.candsAllList = candsList;
    }
  }

  updatePosCandList(cand: ICand) {
    const candsList = [...this.posCandsList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList[index] = { ...cand };
      this.posCandsList = candsList;
    }
  }

  updateFolderCandList(cand: ICand) {
    const candsList = [...this.posCandsList];
    const index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList[index] = { ...cand };
      this.posCandsList = candsList;
    }
  }

  addPosCandList(cand: ICand) {
    const candsList = [...this.posCandsList];
    candsList.unshift({ ...cand });
    this.posCandsList = candsList;
  }

  removePosCandList(cand: ICand) {
    const candsList = [...this.posCandsList];
    const index = this.posCandsList.findIndex(
      (x) => x.candidateId === cand.candidateId
    );
    candsList.splice(index, 1);
    this.posCandsList = candsList;
  }

  async updateCandFoldersIds(candFoldersIdsArr: number[]) {
    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay.candFoldersIds = candFoldersIdsArr;

        let cand = this.candsAllList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.candFoldersIds = candFoldersIdsArr;
        }

        cand = this.posCandsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.candFoldersIds = candFoldersIdsArr;
        }

        cand = this.folderCandsList.find(
          (x) => x.candidateId === this.candDisplay?.candidateId
        );

        if (cand) {
          cand.candFoldersIds = candFoldersIdsArr;
        }
      }
    });
  }

  async updateCandPositionStatus(stageType: string | ICompanyStagesTypes) {
    this.rootStore.generalStore.backdrop = true;

    if (this.candDisplay?.candidateId && this.candDisplay?.position?.id) {
      const res = await this.cvsApi.updateCandPositionStatus(
        stageType.toString(),
        this.candDisplay?.candidateId,
        this.candDisplay?.position?.id
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

  async sendEmail(emailData: ISendEmail) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.sendEmail(emailData);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }
}
