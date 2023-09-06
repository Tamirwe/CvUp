import axios from "axios";
import { IIdName } from "../../models/AuthModels";
import BaseApi from "./BaseApi";

export default class GeneralApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      return await this.http.get("Search");
    });
  }

  async getFileBase64() {
    return await this.apiWrapper(async () => {
      return await this.http.get("Download");
    });
  }

  async addUpdateHrCompany(hrCompany: IIdName) {
    return await this.apiWrapper(async () => {
      return await this.http.post("General/AddUpdateHrCompany", hrCompany);
    });
  }

  async getHrCompaniesList() {
    return await this.apiWrapper<IIdName[]>(async () => {
      return await this.http.get("General/GetHrCompaniesList");
    });
  }

  async deleteHrCompany(id: number) {
    return await this.apiWrapper(async () => {
      return await this.http.delete(`General/DeleteHrCompany?id=${id}`);
    });
  }

  async getIsAuthorized() {
    return await this.apiWrapper2(async () => {
      return await this.http.get(`Cand/GetIsAuthorized`);
    });
  }

  async translateSingleLine(txt: string, lang: string) {
    return await this.apiWrapper2<string>(async () => {
      return await this.http.post(`General/TranslateSingleLine`, { txt, lang });
    });
  }

  async translateMultiLines(txtList: string[], lang: string) {
    return await this.apiWrapper2<string[]>(async () => {
      return await this.http.post(`General/TranslateMultiLines`, {
        txtList,
        lang,
      });
    });
  }
}
