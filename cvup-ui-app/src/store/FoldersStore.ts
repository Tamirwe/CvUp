import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, IFolder, IFolderNode } from "../models/GeneralModels";
import FoldersApi from "./api/FoldersApi";
import { RootStore } from "./RootStore";

export class FoldersStore {
  private foldersApi;
  foldersList: IFolder[] = [];
  private folderSelected?: IFolder;
  private editFolder?: IFolder;
  rootFolder: IFolder = {
    id: 0,
    name: "Folders",
    parentId: -1,
  };

  get selectedFolder() {
    return this.folderSelected;
  }

  set selectedFolder(val: IFolder | undefined) {
    this.folderSelected = val;
  }

  get editFolderSelected() {
    return this.editFolder;
  }

  set editFolderSelected(val: IFolder | undefined) {
    this.editFolder = val;
  }

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.foldersApi = new FoldersApi(appSettings);
  }

  reset() {}

  async addFolder(folderModel: IFolder) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.foldersApi.addFolder(folderModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateFolder(folderModel: IFolder) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.foldersApi.updateFolder(folderModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteFolder(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.foldersApi.deleteFolder(id);
    this.rootStore.generalStore.backdrop = false;
    this.selectedFolder = undefined;
    return response;
  }

  async getFoldersList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.foldersApi.getFoldersList();
    runInAction(() => {
      this.foldersList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async attachCandidate(folderId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.foldersApi.attachCandidate(
      folderId,
      this.rootStore.candsStore.candAllSelected?.candidateId
    );
    this.rootStore.generalStore.backdrop = false;
  }
}
