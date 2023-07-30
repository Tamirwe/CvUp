import { makeAutoObservable, runInAction } from "mobx";
import {
  IAppSettings,
  IFolder,
  IFolderNode,
  ISearchModel,
} from "../models/GeneralModels";
import FoldersApi from "./api/FoldersApi";
import { RootStore } from "./RootStore";
import { numArrRemoveItem } from "../utils/GeneralUtils";
import { TabsCandsEnum } from "../models/GeneralEnums";

export class FoldersStore {
  private foldersApi;
  private foldersList: IFolder[] = [];
  private folderSelected?: IFolder;
  private editFolder?: IFolder;
  private searchPhrase?: ISearchModel;
  rootFolder: IFolder = {
    id: 0,
    name: "Folders",
    parentId: -1,
  };

  get foldersListSorted() {
    if (this.searchPhrase && this.searchPhrase.value) {
      return this.foldersList.filter((x) =>
        x.name.toLowerCase().includes(this.searchPhrase!.value.toLowerCase())
      );
    } else {
      return this.foldersList.slice();
    }
  }

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

  searchFolders(searchVals: ISearchModel) {
    this.searchPhrase = searchVals;
  }

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
    const folderIndex = this.foldersList.findIndex((x) => x.id === id);
    const response = await this.foldersApi.deleteFolder(id);

    if (response.isSuccess) {
      this.foldersList.splice(folderIndex, 1);
      this.rootStore.candsStore.currentTabCandsLists = TabsCandsEnum.AllCands;
      this.selectedFolder = undefined;
    }

    this.rootStore.generalStore.backdrop = false;
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
      this.rootStore.candsStore.candDisplay?.candidateId
    );

    if (
      res.isSuccess &&
      this.rootStore.candsStore.candDisplay?.candFoldersIds
    ) {
      const foldersIdsArr = [
        ...this.rootStore.candsStore.candDisplay?.candFoldersIds,
      ];
      foldersIdsArr.push(folderId);
      this.rootStore.candsStore.updateCandFoldersIds(foldersIdsArr);
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async detachCandidate(folderId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.foldersApi.detachCandidate(
      folderId,
      this.rootStore.candsStore.candDisplay?.candidateId
    );

    if (
      res.isSuccess &&
      this.rootStore.candsStore.candDisplay?.candFoldersIds
    ) {
      const foldersIdsArr = [
        ...this.rootStore.candsStore.candDisplay?.candFoldersIds,
      ];
      numArrRemoveItem(folderId, foldersIdsArr);
      this.rootStore.candsStore.updateCandFoldersIds(foldersIdsArr);
    }

    this.rootStore.generalStore.backdrop = false;
  }
}
