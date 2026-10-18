import {
  IContractTemplate,
  ICustomerContract,
  ISendContract,
} from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class CustomerContractsApi extends BaseApi {
  async getContractTemplates() {
    return await this.apiWrapper2<IContractTemplate[]>(async () => {
      return await this.http.get("CustomerContracts/GetContractTemplates");
    });
  }

  // The only multipart call in the app. The browser sets the multipart
  // boundary itself, so no Content-Type header is passed here.
  async addContractTemplate(name: string, descr: string, file: File) {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("name", name);
    formData.append("descr", descr);

    return await this.apiWrapper2<IContractTemplate>(async () => {
      return await this.http.post(
        "CustomerContracts/AddContractTemplate",
        formData
      );
    });
  }

  async updateContractTemplate(template: IContractTemplate) {
    return await this.apiWrapper2<IContractTemplate>(async () => {
      return await this.http.put(
        "CustomerContracts/UpdateContractTemplate",
        template
      );
    });
  }

  async deleteContractTemplate(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete(
        `CustomerContracts/DeleteContractTemplate?id=${id}`
      );
    });
  }

  async getCustomerLatestContract(customerId: number) {
    return await this.apiWrapper2<ICustomerContract>(async () => {
      return await this.http.get(
        `CustomerContracts/GetCustomerLatestContract?customerId=${customerId}`
      );
    });
  }

  async getLatestContracts() {
    return await this.apiWrapper2<ICustomerContract[]>(async () => {
      return await this.http.get("CustomerContracts/GetLatestContracts");
    });
  }

  async sendContract(data: ISendContract) {
    return await this.apiWrapper2<{ fileName: string }>(async () => {
      return await this.http.post("CustomerContracts/SendContract", data);
    });
  }
}
