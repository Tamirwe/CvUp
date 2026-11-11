import { makeAutoObservable, runInAction } from "mobx";
import {
  IContractSend,
  IContractTemplate,
  IEmailTemplate,
  ISendContract,
} from "../models/GeneralModels";
import ContractTemplatesApi from "./api/ContractTemplatesApi";
import { RootStore } from "./RootStore";

export class ContractTemplatesStore {
  private contractTemplatesApi;
  contractTemplatesList: IContractTemplate[] = [];
  contractEmailTemplates: IEmailTemplate[] = [];
  contractSendsList: IContractSend[] = [];

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

  async getContractEmailTemplates() {
    const res = await this.contractTemplatesApi.getContractEmailTemplates();

    runInAction(() => {
      this.contractEmailTemplates = res.data || [];
    });

    return res;
  }

  async getContractSends() {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.getContractSends();

    runInAction(() => {
      this.contractSendsList = res.data || [];
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

  async sendContract(contract: ISendContract) {
    this.setBackdrop(true);

    const res = await this.contractTemplatesApi.sendContract(contract);

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
