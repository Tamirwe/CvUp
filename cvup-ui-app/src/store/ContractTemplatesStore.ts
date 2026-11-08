import { makeAutoObservable, runInAction } from "mobx";
import { IContractTemplate } from "../models/GeneralModels";
import ContractTemplatesApi from "./api/ContractTemplatesApi";
import { RootStore } from "./RootStore";

export class ContractTemplatesStore {
  private contractTemplatesApi;
  contractTemplatesList: IContractTemplate[] = [];

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.contractTemplatesApi = new ContractTemplatesApi();
  }

  private setBackdrop(val: boolean) {
    runInAction(() => {
      this.rootStore.generalStore.backdrop = val;
    });
  }

  async getContractTemplates() {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.getContractTemplates();

    runInAction(() => {
      this.contractTemplatesList = res.data || [];
    });

    this.setBackdrop(false);

    return res;
  }

  async addContractTemplate(name: string, descr: string, file: File) {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.addContractTemplate(
      name,
      descr,
      file
    );

    this.setBackdrop(false);

    return res;
  }

  async deleteContractTemplate(id: number) {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.deleteContractTemplate(id);

    this.setBackdrop(false);

    return res;
  }

  async getContractTemplateFile(id: number) {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.getContractTemplateFile(id);

    this.setBackdrop(false);

    return res;
  }
}
