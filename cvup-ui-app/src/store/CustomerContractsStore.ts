import { makeAutoObservable, runInAction } from "mobx";
import {
  IContractTemplate,
  ICustomerContract,
  ISendContract,
} from "../models/GeneralModels";
import CustomerContractsApi from "./api/CustomerContractsApi";
import { RootStore } from "./RootStore";

export class CustomerContractsStore {
  private customerContractsApi;

  contractTemplatesList: IContractTemplate[] = [];
  latestContracts: ICustomerContract[] = [];
  customerLatestContract?: ICustomerContract;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.customerContractsApi = new CustomerContractsApi();
  }

  reset() {
    this.contractTemplatesList = [];
    this.latestContracts = [];
    this.customerLatestContract = undefined;
  }

  // A template with no file behind it cannot be merged, so the send form only
  // offers the ones the company folder actually holds.
  get sendableTemplates() {
    return this.contractTemplatesList.filter((x) => x.fileExists);
  }

  async getContractTemplates() {
    const res = await this.customerContractsApi.getContractTemplates();
    runInAction(() => {
      this.contractTemplatesList = res.data ?? [];
    });
    return res;
  }

  async addContractTemplate(name: string, descr: string, file: File) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.customerContractsApi.addContractTemplate(
      name,
      descr,
      file
    );
    this.rootStore.generalStore.backdrop = false;

    if (res.isSuccess) {
      await this.getContractTemplates();
    }

    return res;
  }

  async updateContractTemplate(template: IContractTemplate) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.customerContractsApi.updateContractTemplate(
      template
    );
    this.rootStore.generalStore.backdrop = false;

    if (res.isSuccess) {
      await this.getContractTemplates();
    }

    return res;
  }

  async deleteContractTemplate(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.customerContractsApi.deleteContractTemplate(id);
    this.rootStore.generalStore.backdrop = false;

    if (res.isSuccess) {
      await this.getContractTemplates();
    }

    return res;
  }

  async getCustomerLatestContract(customerId: number) {
    const res = await this.customerContractsApi.getCustomerLatestContract(
      customerId
    );
    runInAction(() => {
      this.customerLatestContract = res.data ?? undefined;
    });
    return res;
  }

  clearCustomerLatestContract() {
    runInAction(() => {
      this.customerLatestContract = undefined;
    });
  }

  async getLatestContracts() {
    const res = await this.customerContractsApi.getLatestContracts();
    runInAction(() => {
      this.latestContracts = res.data ?? [];
    });
    return res;
  }

  async sendContract(data: ISendContract) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.customerContractsApi.sendContract(data);
    this.rootStore.generalStore.backdrop = false;

    if (res.isSuccess) {
      await this.getCustomerLatestContract(data.customerId);
    }

    return res;
  }
}
