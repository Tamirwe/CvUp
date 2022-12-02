import BaseApi from "./BaseApi";

export default class GeneralApi extends BaseApi {
  async search() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Search")).data;
      return data;
    });
  }

  async getFileBase64() {
    return await this.apiWrapper(async () => {
      const data = (await this.http.get("Download")).data;
      return data;
    });
  }
}
