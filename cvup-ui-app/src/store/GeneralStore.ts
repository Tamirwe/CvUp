import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import { EmailTypeEnum, TabsGeneralEnum } from "../models/GeneralEnums";
import { IAppSettings } from "../models/GeneralModels";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class GeneralStore {
  private generalApi;
  customersList: IIdName[] = [];
  hrCompaniesList: IIdName[] = [];
  isShowBackdrop: boolean = false;
  private isCvReviewDialogOpen: boolean = false;
  private showEmailDialogType: EmailTypeEnum = EmailTypeEnum.None;
  currentTabSelectesd: TabsGeneralEnum = TabsGeneralEnum.Positions;
  private isShowFolderFormDialog: boolean = false;
  private isShowContactFormDialog: boolean = false;

  set backdrop(val) {
    this.isShowBackdrop = val;
  }

  get backdrop() {
    return this.isShowBackdrop;
  }

  set cvReviewDialogOpen(val) {
    this.isCvReviewDialogOpen = val;
  }

  get cvReviewDialogOpen() {
    return this.isCvReviewDialogOpen;
  }

  set showEmailDialog(val) {
    this.showEmailDialogType = val;
  }

  get showEmailDialog() {
    return this.showEmailDialogType;
  }

  set currentTab(val) {
    this.currentTabSelectesd = val;
  }

  get currentTab() {
    return this.currentTabSelectesd;
  }

  set showFolderFormDialog(val) {
    this.isShowFolderFormDialog = val;
  }

  get showFolderFormDialog() {
    return this.isShowFolderFormDialog;
  }

  set showContactFormDialog(val) {
    this.isShowContactFormDialog = val;
  }

  get showContactFormDialog() {
    return this.isShowContactFormDialog;
  }

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.generalApi = new GeneralApi(appSettings);
  }

  reset() {
    this.customersList = [];
    this.hrCompaniesList = [];
    this.isShowBackdrop = false;
  }

  async search() {
    const aaa = await this.generalApi.search();
    console.log(aaa);
  }

  async getFileBase64() {
    const aaa = await this.generalApi.getFileBase64();
    return aaa.data;
  }

  async addUpdateCustomer(customer: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.addUpdateCustomer(customer);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getCustomersList(loadAgain: boolean) {
    this.rootStore.generalStore.backdrop = true;
    if (this.customersList.length === 0 || loadAgain) {
      const res = await this.generalApi.getCustomersList();
      runInAction(() => {
        this.customersList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async deleteCustomer(customerId: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.deleteCustomer(customerId);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async addUpdateHrCompany(hrCompany: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.addUpdateHrCompany(hrCompany);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getHrCompaniesList(loadAgain: boolean) {
    this.rootStore.generalStore.backdrop = true;
    if (this.hrCompaniesList.length === 0 || loadAgain) {
      const res = await this.generalApi.getHrCompaniesList();
      runInAction(() => {
        this.hrCompaniesList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async deleteHrCompany(hrCompanyId: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.deleteHrCompany(hrCompanyId);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }
}
