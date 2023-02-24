import { IFolder } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class FoldersApi extends BaseApi {
  async getFoldersList() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IFolder[]>("Folders/GetFoldersList"))
        .data;
      return data;
    });

    return response;
  }
}
