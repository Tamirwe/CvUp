import { IFolder } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class FoldersApi extends BaseApi {
  async addFolder(folderModel: IFolder) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.post("Folders/AddFolder", folderModel))
        .data;
      return response;
    });

    return responseData;
  }

  async updateFolder(folderModel: IFolder) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.put("Folders/UpdateFolder", folderModel))
        .data;
      return response;
    });

    return responseData;
  }

  async deleteFolder(id: number) {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.delete("Folders/DeleteFolder?id=" + id))
        .data;
      return response;
    });

    return responseData;
  }

  async getFoldersList() {
    const responseData = await this.apiWrapper(async () => {
      const response = (await this.http.get<IFolder[]>("Folders/GetFolders")).data;
      return response;
    });

    return responseData;
  }

  async attachCandidate(folderId: number, candidateId: number | undefined) {
    const responseData = await this.apiWrapper(async () => {
      const response = (
        await this.http.post("Folders/AttachCandidate", {
          folderId,
          candidateId,
        })
      ).data;
      return response;
    });

    return responseData;
  }
}
