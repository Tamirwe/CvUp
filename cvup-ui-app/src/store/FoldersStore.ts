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
  private isfoldersListSortDirectionDesc: boolean = true;

  rootFolder: IFolder = {
    id: 0,
    name: "Folders",
    parentId: -1,
  };

  get foldersListSorted() {
    let posList;

    if (this.searchPhrase && this.searchPhrase.value) {
      posList = this.foldersList.filter((x) =>
        x.name.toLowerCase().includes(this.searchPhrase!.value.toLowerCase())
      );
    } else {
      posList = this.foldersList.slice();
    }

    if (this.isfoldersListSortDirectionDesc) {
      posList.sort((a, b) => (a.name > b.name ? 1 : b.name > a.name ? -1 : 0));
    } else {
      posList.sort((a, b) => (b.name > a.name ? 1 : a.name > b.name ? -1 : 0));
    }

    return posList;
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

  set foldersListSortDirection(val: string) {
    this.isfoldersListSortDirectionDesc = val === "desc" ? true : false;
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

    if (res.isSuccess) {
      await this.rootStore.candsStore.getFolderCandsList();
      this.rootStore.candsStore.updateCandListFolderAttached(res.data);
    }
    
    this.rootStore.generalStore.backdrop = false;
  }

  async detachCandidate(folderId: number) {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.foldersApi.detachCandidate(
      folderId,
      this.rootStore.candsStore.candDisplay?.candidateId
    );

    if (res.isSuccess) {
      await this.rootStore.candsStore.getFolderCandsList();
      this.rootStore.candsStore.updateCandListFolderAttached(res.data);
    }

    this.rootStore.generalStore.backdrop = false;
  }
}
