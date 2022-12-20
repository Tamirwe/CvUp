import { makeAutoObservable, runInAction } from "mobx";
import { IIdName } from "../models/AuthModels";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class GeneralStore {
  private generalApi;
  departmentsList: IIdName[] | undefined;
  hrCompaniesList: IIdName[] | undefined;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.generalApi = new GeneralApi();
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
    return await this.generalApi.addUpdateDepartment(department);
  }

  async getDepartmentsList(loadAgain: boolean) {
    if (!this.departmentsList || loadAgain) {
      const res = await this.generalApi.getDepartmentsList();
      runInAction(() => {
        this.departmentsList = res.data;
      });
    }
  }

  async deleteDepartment(department: IIdName) {
    return await this.generalApi.deleteDepartment(department);
  }

  async addUpdateHrCompany(hrCompany: IIdName) {
    return await this.generalApi.addUpdateHrCompany(hrCompany);
  }

  async getHrCompaniesList(loadAgain: boolean) {
    if (!this.hrCompaniesList || loadAgain) {
      const res = await this.generalApi.getHrCompaniesList();
      runInAction(() => {
        this.hrCompaniesList = res.data;
      });
    }
  }

  async deleteHrCompany(hrCompany: IIdName) {
    return await this.generalApi.deleteHrCompany(hrCompany);
  }
}
