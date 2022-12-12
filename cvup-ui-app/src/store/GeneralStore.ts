import { makeAutoObservable } from "mobx";
import { IDepartment, IHrCompany } from "../models/AuthModels";
import GeneralApi from "./api/GeneralApi";
import { RootStore } from "./RootStore";

export class GeneralStore {
  private generalApi;

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

  async addUpdateHrCompany(hrCompany: IHrCompany) {
    return await this.generalApi.addUpdateHrCompany(hrCompany);
  }

  async addUpdateDepartment(department: IDepartment) {
    return await this.generalApi.addUpdateDepartment(department);
  }
}
