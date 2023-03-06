import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, IFolder } from "../models/GeneralModels";
import FoldersApi from "./api/FoldersApi";
import { RootStore } from "./RootStore";

export class FoldersStore {
  private foldersApi;
  foldersList: IFolder[] = [];
  folderSelected?: IFolder;

  get selectedFolder() {
    return this.folderSelected;
  }

  set selectedFolder(val: IFolder | undefined) {
    this.folderSelected = val;
  }

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.foldersApi = new FoldersApi(appSettings);
  }

  reset() {}

  async addFolder(folderModel: IFolder) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.foldersApi.addFolder(folderModel);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async updateFolder(folderModel: IFolder) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.foldersApi.updateFolder(folderModel);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getFoldersList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.foldersApi.getFoldersList();
    runInAction(() => {
      this.foldersList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }
}
