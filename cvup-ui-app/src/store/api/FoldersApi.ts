import { ICand, IFolder } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class FoldersApi extends BaseApi {
  async addFolder(folderModel: IFolder) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Folders/AddFolder", folderModel);
    });
  }

  async updateFolder(folderModel: IFolder) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Folders/UpdateFolder", folderModel);
    });
  }

  async deleteFolder(id: number) {
    return await this.apiWrapper2(async () => {
      return await this.http.delete("Folders/DeleteFolder?id=" + id);
    });
  }

  async getFoldersList() {
    return await this.apiWrapper2<IFolder[]>(async () => {
      return await this.http.get("Folders/GetFolders");
    });
  }

  async attachCandidate(folderId: number, candidateId: number | undefined) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post("Folders/AttachCandidate", {
        folderId,
        candidateId,
      });
    });
  }

  async detachCandidate(folderId: number, candidateId: number | undefined) {
    return await this.apiWrapper2<ICand>(async () => {
      return await this.http.post("Folders/DetachCandidate", {
        folderId,
        candidateId,
      });
    });
  }
}
