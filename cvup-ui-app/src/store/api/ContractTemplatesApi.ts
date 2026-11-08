import { IContractTemplate } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class ContractTemplatesApi extends BaseApi {
  async getContractTemplates() {
    return await this.apiWrapper2<IContractTemplate[]>(async () => {
      return await this.http.get("ContractTemplates/GetContractTemplates");
    });
  }

  async addContractTemplate(name: string, descr: string, file: File) {
    const formData = new FormData();

    formData.append("name", name);
    formData.append("descr", descr);
    formData.append("file", file);

    return await this.apiWrapper2<IContractTemplate>(async () => {
      return await this.http.post(
        "ContractTemplates/AddContractTemplate",
        formData,
        { timeout: 120000 }
      );
    });
  }

  async deleteContractTemplate(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("ContractTemplates/DeleteContractTemplate", {
        params: { id },
      });
    });
  }

  async getContractTemplateFile(id: number) {
    return await this.apiWrapper2<Blob>(async () => {
      return await this.http.get("ContractTemplates/GetContractTemplateFile", {
        params: { id },
        responseType: "blob",
        timeout: 30000,
      });
    });
  }
}
