import { makeAutoObservable, runInAction } from "mobx";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";
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
  private searchesList: ISearchModel[] = [];
  private externalSearch?: ISearchModel;
  private isShoePosStages?: boolean = false;
  private lastReviewCandId: number = 0;
  private syncCandReview: string = "";
  searchesSearchVals?: ISearchModel;
  // private isPdfLoaded: boolean = false;

  allCandsList: ICand[] = [];
  candDupCvsList: ICandCv[] = [];
  posCandsList: ICand[] = [];
  posTypeCandsList: ICand[] = [];
  folderCandsList: ICand[] = [];
  pdfUrl: string = "";
  pdfBlobUrl: string = "";
  sortedSearchesList: ISearchModel[] = [];

  // candAllSelected?: ICand;
  candDupSelected?: ICandCv;
  // candPosSelected?: ICand;
  // candFolderSelected?: ICand;
  candDisplay?: ICand;
  candDisplaySource: CandsSourceEnum = 0;
  private tabDisplayCandsLists: TabsCandsEnum = TabsCandsEnum.AllCands;
  posStages?: IPosStagesType[];
  emailTemplates?: IEmailTemplate[];
  candsReportData?: ICandsReport[];
  lastSearchVals: string = "";

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
    // this.candAllSelected = undefined;
  }

  set currentTabCandsLists(val) {
    this.tabDisplayCandsLists = val;
  }

  get currentTabCandsLists() {
    return this.tabDisplayCandsLists;
  }

  // set pdfLoaded(val) {
  //   this.isPdfLoaded = val;
  // }

  get downloadUrl() {
    return this.appSettings.apiUrl;
  }

  set extSearch(val) {
    this.externalSearch = val;
  }

  get extSearch() {
    return this.externalSearch;
  }

  set shoePosStages(val) {
    this.isShoePosStages = val;
  }

  get shoePosStages() {
    return this.isShoePosStages;
  }

  get keywordsHighLight() {
    const keywordsarray = this.lastSearchVals.replaceAll('"', " ");

    return keywordsarray;
  }

  get candReviewSync() {
    return this.syncCandReview;
  }

  set candReviewSync(val) {
    this.syncCandReview = val;
  }

  async displayCv(cand: ICand, candsSource: CandsSourceEnum) {
    //in mobile when cand review take all screen pdf viewr not load cv because it not in screan
    // if (isMobile) {
    //   this.candDisplay = undefined;
    // }

    // if (
    //   this.candDisplay?.candidateId !== cand.candidateId ||
    //   this.candDisplay?.cvId !== cand.cvId
    // ) {
    await this.getPdf(cand.keyId);
    // }

    runInAction(() => {
      this.candDisplay = cand;
      this.candDisplaySource = candsSource;
      this.rootStore.positionsStore.setRelatedPositionToCandDisplay(
        candsSource
      );

      if (
        this.rootStore.authStore.currentUser?.email !== "tamir.we+a1@gmail.com"
      ) {
        if (!cand.isSeen) {
          this.cvsApi.updateIsSeen(cand.cvId);
          cand.isSeen = true;

          //not must but any way
          const updatedCand = Object.assign({}, cand);
          this.updateLists(updatedCand);
        }
      }
    });
  }

  async nexePrevCv(indexAdd: number) {
    let list;

    switch (this.candDisplaySource) {
      case CandsSourceEnum.Folder:
        list = this.folderCandsList;

        break;
      case CandsSourceEnum.Position:
        list = this.posCandsList;

        break;
      case CandsSourceEnum.PositionType:
        list = this.posTypeCandsList;

        break;
      default:
        list = this.allCandsList;
        break;
    }

    if (list) {
      let ind = list?.findIndex(
        (x) => x.candidateId === this.candDisplay?.candidateId
      );

      if (ind > -1) {
        ind = ind + indexAdd;

        if (ind > -1 && ind < list?.length) {
          this.displayCv(list[ind], this.candDisplaySource);
        }
      }
    }
  }

  async displayCvDuplicate(candCv: ICandCv) {
    runInAction(() => {
      this.candDupSelected = candCv;

      if (this.candDisplay) {
        this.candDisplay!.cvId = candCv.cvId;
        this.candDisplay!.keyId = candCv.keyId;
      }

      // switch (listCource) {
      //   case CvDisplayedListEnum.CandsList:
      //     this.candAllSelected = this.allCandsList.find(
      //       (x) => x.candidateId === candCv.candidateId
      //     );
      //     break;
      //   case CvDisplayedListEnum.PositionCandsList:
      //     this.candPosSelected = this.posCandsList.find(
      //       (x) => x.candidateId === candCv.candidateId
      //     );
      //     break;
      //   case CvDisplayedListEnum.FolderCandsList:
      //     this.candFolderSelected = this.folderCandsList.find(
      //       (x) => x.candidateId === candCv.candidateId
      //     );
      //     break;
      //   default:
      //     break;
      // }

      this.getPdf(candCv.keyId);
    });
  }

  async getPdf(keyId: string) {
    const data = await this.getPdfFile(keyId);
    if (!(data instanceof Blob)) return;
    const downloadedFile = new Blob([data!], {
      type: data.type,
    });

    runInAction(() => {
      this.pdfBlobUrl = URL.createObjectURL(downloadedFile);
    });

    // this.pdfUrl = `${this.appSettings.apiUrl}DD?id=${keyId}`;
  }

  async saveCandReview(review: string) {
    this.rootStore.generalStore.backdrop = true;
    this.rootStore.generalStore.startSpinnerTimeout();

    const res = await this.cvsApi.saveCandReview(
      review,
      this.candDisplay?.candidateId!
    );

    if (res.isSuccess && res.data) {
      runInAction(() => {
        this.candDisplay = { ...res.data };
        this.updateLists(res.data);
      });
    }

    this.rootStore.generalStore.backdrop = false;
    return res;
  }

  saveReviewToLocalStorage(review: string) {
    if (this.candDisplay?.candidateId !== this.lastReviewCandId) {
      localStorage.setItem(
        "PrevReview",
        localStorage.getItem("LastReview") || ""
      );
      localStorage.setItem(
        "PrevReviewCandDetails",
        localStorage.getItem("LastReviewCandDetails") || ""
      );
    }

    this.lastReviewCandId = this.candDisplay?.candidateId!;

    localStorage.setItem("LastReview", review);
    localStorage.setItem(
      "LastReviewCandDetails",
      JSON.stringify({
        candId: this.candDisplay?.candidateId!,
        positionId: this.rootStore.positionsStore.candDisplayPosition?.id,
        firstName: this.candDisplay?.firstName,
        lastName: this.candDisplay?.lastName,
        customerName:
          this.rootStore.positionsStore.candDisplayPosition?.customerName,
        positionName: this.rootStore.positionsStore.candDisplayPosition?.name,
      })
    );
  }

  async saveCustomerCandReview(review: string) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.saveCustomerCandReview(
      review,
      this.candDisplay?.candidateId!,
      this.rootStore.positionsStore.candDisplayPosition?.id
    );

    if (res.isSuccess && res.data) {
      runInAction(() => {
        this.candDisplay = { ...res.data };
        this.updateLists(res.data);
      });
    }

    this.rootStore.generalStore.backdrop = false;
    return res;
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
          this.updateLists(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async searchAllCands(searchVals: ISearchModel) {
    const searchKeywords = searchVals.value.trim();

    if (searchKeywords) {
      this.rootStore.generalStore.backdrop = true;
      const res = await this.cvsApi.searchCands(searchVals, 0, 0, 0);
      runInAction(() => {
        this.allCandsList = res.data;
      });
      this.rootStore.generalStore.backdrop = false;

      this.saveSearch(searchVals);
    }

    this.lastSearchVals = searchKeywords;
  }

  async searchPositionCands(searchVals: ISearchModel) {
    const searchKeywords = searchVals.value.trim();

    if (searchKeywords) {
      this.rootStore.generalStore.backdrop = true;
      const res = await this.cvsApi.searchCands(
        searchVals,
        this.rootStore.positionsStore.selectedPosition?.id,
        0,
        0
      );
      runInAction(() => {
        this.posCandsList = res.data;
      });
      this.rootStore.generalStore.backdrop = false;

      this.saveSearch(searchVals);
    }

    this.lastSearchVals = searchKeywords;
  }

  async searchPositionTypeCands(searchVals: ISearchModel) {
    const searchKeywords = searchVals.value.trim();

    if (searchKeywords) {
      this.rootStore.generalStore.backdrop = true;
      const res = await this.cvsApi.searchCands(
        searchVals,
        0,
        this.rootStore.positionsStore.selectedPositionType?.id,
        0
      );
      runInAction(() => {
        this.posTypeCandsList = res.data;
      });
      this.rootStore.generalStore.backdrop = false;

      this.saveSearch(searchVals);
    }

    this.lastSearchVals = searchKeywords;
  }

  async searchFolderCands(searchVals: ISearchModel) {
    const searchKeywords = searchVals.value.trim();

    if (searchKeywords) {
      this.rootStore.generalStore.backdrop = true;
      const res = await this.cvsApi.searchCands(
        searchVals,
        0,
        0,
        this.rootStore.foldersStore.selectedFolder?.id
      );
      runInAction(() => {
        this.folderCandsList = res.data;
      });
      this.rootStore.generalStore.backdrop = false;

      this.saveSearch(searchVals);
    }

    this.lastSearchVals = searchKeywords;
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
      const res = await this.cvsApi.getDuplicatesCvsList(cand.candidateId);

      runInAction(() => {
        this.duplicateCvsCandId = cand.candidateId;
        this.candDupCvsList = res.data;

        const candDup = this.candDupCvsList.find((x) => x.cvId === cand.cvId);
        this.candDupSelected = candDup;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionCandsList(posId: number) {
    runInAction(() => {
      this.rootStore.generalStore.backdrop = true;
    });

    // runInAction(async () => {
    this.posCandsList = [];

    const res = await this.cvsApi.getPosCandsList(posId);
    runInAction(() => {
      this.posCandsList = res.data;

      this.rootStore.generalStore.backdrop = false;
    });
  }

  async getPositionTypeCandsList(posTypeId: number) {
    runInAction(() => {
      this.rootStore.generalStore.backdrop = true;
    });

    // runInAction(async () => {
    this.posCandsList = [];

    const res = await this.cvsApi.getPosTypeCandsList(posTypeId);

    runInAction(() => {
      this.posTypeCandsList = res.data;

      this.rootStore.generalStore.backdrop = false;
    });
  }

  setDisplayCandOntopPCList() {
    runInAction(() => {
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
          this.updateLists(res.data);
          this.rootStore.positionsStore.setRelatedPositionToCandDisplay();
        }
      });
    }
  }

  async detachPosCand(detachCand?: ICand, positionId?: number) {
    if (detachCand && positionId) {
      const res = await this.cvsApi.detachPosCand(
        detachCand.candidateId,
        detachCand.cvId,
        positionId
      );

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateLists(res.data);

          if (
            this.rootStore.positionsStore.selectedPosition?.id === positionId
          ) {
            this.removePosCandList(res.data);
            this.rootStore.positionsStore.removeCandDisplayPosition();
          }
        }
      });
    }
  }

  updateLists(cand: ICand) {
    this.updateCandAllList(cand);
    this.updatePosCandList(cand);
    this.updateFolderCandList(cand);
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
      this.folderCandsList = candsList;
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

  async updateCandListFolderAttached(cand: ICand) {
    runInAction(() => {
      if (this.candDisplay) {
        this.candDisplay = { ...cand };
        //this.updateLists(cand);
      }
    });
  }

  async updateCandPositionStatus(stageType: string) {
    this.rootStore.generalStore.backdrop = true;
    const candDisplayPosition =
      this.rootStore.positionsStore.candDisplayPosition;

    if (this.candDisplay?.candidateId && candDisplayPosition?.id) {
      const res = await this.cvsApi.updateCandPositionStatus({
        stageType,
        candidateId: this.candDisplay?.candidateId,
        positionId: candDisplayPosition?.id,
      });

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateLists(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async updatePosStageDate(stageType: string, newDate: Date) {
    this.rootStore.generalStore.backdrop = true;
    const candDisplayPosition =
      this.rootStore.positionsStore.candDisplayPosition;

    if (this.candDisplay?.candidateId && candDisplayPosition?.id) {
      const res = await this.cvsApi.updatePosStageDate({
        stageType,
        candidateId: this.candDisplay?.candidateId,
        positionId: candDisplayPosition?.id,
        newDate,
      });

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateLists(res.data);
        }
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async removePosStage(stageType: string) {
    this.rootStore.generalStore.backdrop = true;
    const candDisplayPosition =
      this.rootStore.positionsStore.candDisplayPosition;

    if (this.candDisplay?.candidateId && candDisplayPosition?.id) {
      const res = await this.cvsApi.removePosStage({
        stageType,
        candidateId: this.candDisplay?.candidateId,
        positionId: candDisplayPosition?.id,
      });

      runInAction(() => {
        if (res.isSuccess && res.data) {
          this.candDisplay = { ...res.data };
          this.updateLists(res.data);
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
      .sort((a, b) => (a._dt < b._dt ? 1 : b._dt < a._dt ? -1 : 0));
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

  async sendEmailToContact(emailData: ISendEmail) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.sendEmail(emailData);

    if (res.isSuccess) {
      this.updateCandPositionStatus("cv_sent_to_customer");
    }

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

  async deleteCv() {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.deleteCv(
      this.candDisplay?.candidateId,
      this.candDisplay?.cvId
    );

    if (res.isSuccess) {
      if (res.data && res.data.candidateId) {
        this.updateLists(res.data);
        this.duplicateCvsCandId = 0;
      } else {
        this.removeCandFromLists(this.candDisplay?.candidateId);
      }

      runInAction(() => {
        this.candDisplay = undefined;
      });
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async deleteCandidate() {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.cvsApi.deleteCandidate(
      this.candDisplay?.candidateId
    );

    runInAction(() => {
      if (res.isSuccess) {
        this.removeCandFromLists(this.candDisplay?.candidateId);
        this.candDisplay = undefined;
      }
    });

    this.rootStore.generalStore.backdrop = false;
  }

  removeCandFromLists(candId?: number) {
    let candsList = [...this.allCandsList];
    let index = candsList.findIndex((x) => x.candidateId === candId);

    if (index > -1) {
      candsList.splice(index, 1);
      this.allCandsList = candsList;
    }

    candsList = [...this.posCandsList];
    index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList.splice(index, 1);
      this.posCandsList = candsList;
    }

    candsList = [...this.folderCandsList];
    index = candsList.findIndex(
      (x) => x.candidateId === this.candDisplay?.candidateId
    );

    if (index > -1) {
      candsList.splice(index, 1);
      this.posCandsList = candsList;
    }
  }

  async getfile(keyId?: string) {
    if (keyId) {
      const res = await this.cvsApi.getfile(keyId);
      return res.data;
    }
  }

  async getPdfFile(keyId?: string) {
    if (keyId) {
      const res = await this.cvsApi.getPdfFile(keyId);
      return res.data;
    }
  }

  async getSearches() {
    const res = await this.cvsApi.getSearches();
    this.searchesList = res.data;
    this.sortedSearchesList = [...res.data];
  }

  findStarSearches() {
    const searchVals = Object.assign({}, this.searchesSearchVals);
    searchVals.star = !searchVals.star;
    this.findSearches(searchVals);
  }

  findSearches(searchVals?: ISearchModel) {
    this.searchesSearchVals = searchVals;

    runInAction(() => {
      const val = searchVals?.value;
      let list;

      if (searchVals?.star) {
        list = this.searchesList.filter((x) => x.star == true);
      } else {
        list = [...this.searchesList];
      }

      if (val) {
        this.sortedSearchesList = list.filter(
          (x) =>
            x.value.toLowerCase().includes(val.toLowerCase()) ||
            (x.advancedValue &&
              x.advancedValue.toLowerCase().includes(val.toLowerCase()))
        );
      } else {
        this.sortedSearchesList = list;
      }
    });
  }

  findSearchIndex(searchVals: ISearchModel) {
    let objIndex = -1;

    if (searchVals.id) {
      objIndex = this.searchesList.findIndex((x) => x.id === searchVals.id);
    } else {
      const val = searchVals.value;
      const adv = searchVals.advancedValue;

      objIndex = this.searchesList.findIndex(
        (x) => x.value === val && x.advancedValue == adv
      );
    }

    return objIndex;
  }

  saveSearch(searchVals: ISearchModel) {
    const val = searchVals.value;
    const list = [...this.searchesList];

    if (val) {
      const objIndex = this.findSearchIndex(searchVals);

      if (objIndex > -1) {
        let searchItem = list.splice(objIndex, 1)[0];
        searchItem.updated = new Date();
        list.splice(0, 0, searchItem);
      } else {
        let searchItem = Object.assign({}, searchVals);
        searchItem.updated = new Date();
        list.splice(0, 0, searchItem);
      }

      this.searchesList = list;

      this.cvsApi.saveSearch(searchVals);
    }
  }

  starSearch(searchVals: ISearchModel) {
    const list = [...this.searchesList];

    const objIndex = this.findSearchIndex(searchVals);

    if (objIndex > -1) {
      let searchItem = list[objIndex];
      searchItem.star = !searchItem.star;
    }

    this.searchesList = list;
    this.findSearches(this.searchesSearchVals);
    this.cvsApi.starSearch(searchVals);
  }

  async deleteSearch(searchVals: ISearchModel) {
    const list = [...this.searchesList];

    const objIndex = this.findSearchIndex(searchVals);

    if (objIndex > -1) {
      list.splice(objIndex, 1);
    }

    this.searchesList = list;
    this.findSearches(this.searchesSearchVals);
    this.cvsApi.deleteSearch(searchVals);
  }

  async deleteAllNotStarSearches() {
    const res = await this.cvsApi.deleteAllNotStarSearches();

    this.searchesList = res.data;
    this.sortedSearchesList = [...res.data];
  }
}
