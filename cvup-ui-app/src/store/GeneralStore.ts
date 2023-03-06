import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import {
  CrudTypesEnum,
  EmailTypeEnum,
  TabsGeneralEnum,
} from "../models/GeneralEnums";
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
  private FolderFormDialogModeOpen: CrudTypesEnum = CrudTypesEnum.None;
  private isShowContactFormDialog: boolean = false;
  private isConfirmDialogOpen: boolean = false;
  private confirmDialogResolve?: (isOk: boolean | PromiseLike<boolean>) => void;
  confirmDialogTitle: string = "";
  confirmDialogMessage: string = "";

  get confirmDialogOpen() {
    return this.isConfirmDialogOpen;
  }

  set confirmDialogOpen(val) {
    this.isConfirmDialogOpen = val;
  }

  get backdrop() {
    return this.isShowBackdrop;
  }

  set backdrop(val) {
    this.isShowBackdrop = val;
  }

  get cvReviewDialogOpen() {
    return this.isCvReviewDialogOpen;
  }
  set cvReviewDialogOpen(val) {
    this.isCvReviewDialogOpen = val;
  }

  get showEmailDialog() {
    return this.showEmailDialogType;
  }
  set showEmailDialog(val) {
    this.showEmailDialogType = val;
  }

  get currentTab() {
    return this.currentTabSelectesd;
  }

  set currentTab(val) {
    this.currentTabSelectesd = val;
  }

  get openModeFolderFormDialog() {
    return this.FolderFormDialogModeOpen;
  }

  set openModeFolderFormDialog(val: CrudTypesEnum) {
    this.FolderFormDialogModeOpen = val;
  }

  get showContactFormDialog() {
    return this.isShowContactFormDialog;
  }

  set showContactFormDialog(val) {
    this.isShowContactFormDialog = val;
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

  confirmDialog(title: string, message: string) {
    this.confirmDialogTitle = title;
    this.confirmDialogMessage = message;
    this.isConfirmDialogOpen = true;

    const confirmPromise = () =>
      new Promise<boolean>((resolve, reject) => {
        runInAction(() => {
          this.confirmDialogResolve = resolve;
        });
      });

    return confirmPromise;
  }

  async confirmResponse(isOk: boolean) {
    runInAction(() => {
      if (this.confirmDialogResolve) {
        this.confirmDialogResolve(isOk);
      }
    });
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
