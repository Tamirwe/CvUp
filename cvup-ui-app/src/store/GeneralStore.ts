import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import { EmailTypeEnum } from "../models/GeneralEnums";
import { IAppSettings } from "../models/GeneralModels";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class GeneralStore {
  private generalApi;
  departmentsList: IIdName[] = [];
  hrCompaniesList: IIdName[] = [];
  isShowBackdrop: boolean = false;
  private isCvReviewDialogOpen: boolean = false;
  private showEmailDialogType: EmailTypeEnum = EmailTypeEnum.None;

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

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.generalApi = new GeneralApi(appSettings);
  }

  reset() {
    this.departmentsList = [];
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

  async addUpdateDepartment(department: IIdName) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.addUpdateDepartment(department);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getDepartmentsList(loadAgain: boolean) {
    this.rootStore.generalStore.backdrop = true;
    if (this.departmentsList.length === 0 || loadAgain) {
      const res = await this.generalApi.getDepartmentsList();
      runInAction(() => {
        this.departmentsList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async deleteDepartment(departmentId: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.generalApi.deleteDepartment(departmentId);
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
