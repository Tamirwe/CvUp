import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import {
  AlertConfirmDialogEnum,
  AppModeEnum,
  CrudTypesEnum,
  EmailTypeEnum,
  TabsGeneralEnum,
} from "../models/GeneralEnums";
import { IAppSettings } from "../models/GeneralModels";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";
import { AlertColor } from "@mui/material";

export class GeneralStore {
  private generalApi;
  hrCompaniesList: IIdName[] = [];
  isShowBackdrop: boolean = false;
  private isCvReviewDialogOpen: boolean = false;
  private showEmailDialogType: EmailTypeEnum = EmailTypeEnum.None;
  private currentTabSelectesd: TabsGeneralEnum = TabsGeneralEnum.Positions;
  private FolderFormDialogModeOpen: CrudTypesEnum = CrudTypesEnum.None;
  private isShowContactFormDialog: boolean = false;
  private isShowCustomersListDialog: boolean = false;
  private isShowUserFormDialog: boolean = false;
  private isShowUserListDialog: boolean = false;
  private isAlertConfirmDialogOpen: boolean = false;
  private isPositionFormDialogOpen: boolean = false;
  private isShowReviewCandDialog: boolean = false;
  private isShowCustomerReviewCandDialog: boolean = false;
  private isShowEmailTemplatesDialog: boolean = false;
  private isShowCandFormDialog: boolean = false;
  private isAlertSnackbarOpen: boolean = false;
  private isLeftDrawerOpen: boolean = false;
  private isRightDrawerOpen: boolean = false;
  private confirmDialogResolve?: (isOk: boolean | PromiseLike<boolean>) => void;
  private isshowSearchesListDialog: boolean = false;

  alertConfirmDialogType: AlertConfirmDialogEnum = AlertConfirmDialogEnum.Alert;
  alertConfirmDialogTitle: string = "";
  alertConfirmDialogMessage: string = "";
  private appModeType: AppModeEnum = AppModeEnum.HRCompany;
  alertSnackbarType?: AlertColor = "success";
  alertSnackbarMessage = "";

  get appMode() {
    return this.appModeType;
  }

  get alertConfirmDialogOpen() {
    return this.isAlertConfirmDialogOpen;
  }

  set alertConfirmDialogOpen(val) {
    this.isAlertConfirmDialogOpen = val;
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

  get showPositionFormDialog() {
    return this.isPositionFormDialogOpen;
  }

  set showPositionFormDialog(val) {
    this.isPositionFormDialogOpen = val;
  }

  get leftDrawerOpen() {
    return this.isLeftDrawerOpen;
  }

  set leftDrawerOpen(val) {
    this.isLeftDrawerOpen = val;
  }

  get rightDrawerOpen() {
    return this.isRightDrawerOpen;
  }

  set rightDrawerOpen(val) {
    this.isRightDrawerOpen = val;
  }

  get currentLeftDrawerTab() {
    return this.currentTabSelectesd;
  }

  set currentLeftDrawerTab(val) {
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

  get showCustomersListDialog() {
    return this.isShowCustomersListDialog;
  }

  set showCustomersListDialog(val) {
    this.isShowCustomersListDialog = val;
  }

  get showUserFormDialog() {
    return this.isShowUserFormDialog;
  }

  set showUserFormDialog(val) {
    this.isShowUserFormDialog = val;
  }

  get showUserListDialog() {
    return this.isShowUserListDialog;
  }

  set showUserListDialog(val) {
    this.isShowUserListDialog = val;
  }

  get showReviewCandDialog() {
    return this.isShowReviewCandDialog;
  }

  set showReviewCandDialog(val) {
    this.isShowReviewCandDialog = val;
  }

  get showCustomerReviewCandDialog() {
    return this.isShowCustomerReviewCandDialog;
  }

  set showCustomerReviewCandDialog(val) {
    this.isShowCustomerReviewCandDialog = val;
  }

  get showEmailTemplatesDialog() {
    return this.isShowEmailTemplatesDialog;
  }

  set showEmailTemplatesDialog(val) {
    this.isShowEmailTemplatesDialog = val;
  }

  get showCandFormDialog() {
    return this.isShowCandFormDialog;
  }

  set showCandFormDialog(val) {
    this.isShowCandFormDialog = val;
  }

  get alertSnackbarOpen() {
    return this.isAlertSnackbarOpen;
  }

  set alertSnackbarOpen(val) {
    this.isAlertSnackbarOpen = val;
    this.alertSnackbarMessage = "";
  }

  get showSearchesListDialog() {
    return this.isshowSearchesListDialog;
  }

  set showSearchesListDialog(val) {
    this.isshowSearchesListDialog = val;
  }

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.generalApi = new GeneralApi(appSettings);
    this.appModeType =
      AppModeEnum[appSettings.appMode as keyof typeof AppModeEnum];
  }

  reset() {
    this.hrCompaniesList = [];
    this.isShowBackdrop = false;
  }

  alertConfirmDialog(
    type: AlertConfirmDialogEnum,
    title: string,
    message: string
  ) {
    this.alertConfirmDialogType = type;
    this.alertConfirmDialogTitle = title;
    this.alertConfirmDialogMessage = message;
    this.isAlertConfirmDialogOpen = true;

    const confirmPromise = () =>
      new Promise<boolean>((resolve, reject) => {
        runInAction(() => {
          this.confirmDialogResolve = resolve;
        });
      });

    return confirmPromise();
  }

  async confirmResponse(isOk: boolean) {
    runInAction(() => {
      if (this.confirmDialogResolve) {
        this.confirmDialogResolve(isOk);
      }
    });
  }

  alertSnackbar(alertType: AlertColor, message: string) {
    this.alertSnackbarType = alertType;
    this.alertSnackbarMessage = message;
    this.isAlertSnackbarOpen = true;
  }

  async search() {
    const aaa = await this.generalApi.search();
    console.log(aaa);
  }

  async getFileBase64() {
    const aaa = await this.generalApi.getFileBase64();
    return aaa.data;
  }

  async getIsAuthorized() {
    const res = await this.generalApi.getIsAuthorized();
    return res;
  }

  // async addUpdateCustomer(customer: IIdName) {
  //   this.rootStore.generalStore.backdrop = true;
  //   const response = await this.generalApi.addUpdateCustomer(customer);
  //   this.rootStore.generalStore.backdrop = false;
  //   return response;
  // }

  // async getCustomersList(loadAgain: boolean) {
  //   this.rootStore.generalStore.backdrop = true;
  //   if (this.customersList.length === 0 || loadAgain) {
  //     const res = await this.generalApi.getCustomersList();
  //     runInAction(() => {
  //       this.customersList = res.data;
  //     });
  //   }
  //   this.rootStore.generalStore.backdrop = false;
  // }

  // async deleteCustomer(customerId: number) {
  //   this.rootStore.generalStore.backdrop = true;
  //   const response = await this.generalApi.deleteCustomer(customerId);
  //   this.rootStore.generalStore.backdrop = false;
  //   return response;
  // }

  async addUpdateHrCompany(hrCompany: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.generalApi.addUpdateHrCompany(hrCompany);
    this.rootStore.generalStore.backdrop = false;
    return response;
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
    const response = await this.generalApi.deleteHrCompany(hrCompanyId);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
