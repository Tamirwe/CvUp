import { IFolder } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class FoldersApi extends BaseApi {
  async addFolder(folderModel: IFolder) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Folders/AddFolder", folderModel))
        .data;
      return data;
    });

    return response;
  }

  async updateFolder(folderModel: IFolder) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.post("Folders/UpdateFolder", folderModel))
        .data;
      return data;
    });

    return response;
  }

  async deleteFolder(id: number) {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.delete("Folders/DeleteFolder?id=" + id))
        .data;
      return data;
    });

    return response;
  }

  async getFoldersList() {
    const response = await this.apiWrapper(async () => {
      const data = (await this.http.get<IFolder[]>("Folders/GetFolders")).data;
      return data;
    });

    return response;
  }
}
