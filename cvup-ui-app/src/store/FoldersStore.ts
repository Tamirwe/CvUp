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
  foldersList: IFolder[] = [];
  private folderSelected?: IFolder;
  private editFolder?: IFolder;
  private searchPhrase?: ISearchModel;
  private isfoldersListSortDirectionDesc: boolean = true;
  sortedFolders: IFolder[] = [];

  rootFolder: IFolder = {
    id: 0,
    name: "Folders",
    parentId: -1,
  };

  // get foldersListSorted() {
  //   let posList;

  //   if (this.searchPhrase && this.searchPhrase.value) {
  //     posList = this.foldersList.filter((x) =>
  //       x.name.toLowerCase().includes(this.searchPhrase!.value.toLowerCase())
  //     );
  //   } else {
  //     posList = this.foldersList.slice();
  //   }

  //   if (this.isfoldersListSortDirectionDesc) {
  //     posList.sort((a, b) => (a.name > b.name ? 1 : b.name > a.name ? -1 : 0));
  //   } else {
  //     posList.sort((a, b) => (b.name > a.name ? 1 : a.name > b.name ? -1 : 0));
  //   }

  //   return posList;
  // }

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
    let searchedFoldersList: IFolder[] = [];

    if (searchVals && searchVals.value) {
      let filteredList: IFolder[];

      filteredList = this.foldersList.filter((x) =>
        x.name.toLowerCase().includes(searchVals!.value.toLowerCase())
      );

      const topParents: IFolder[] = [];
      for (let i = 0; i < filteredList.length; i++) {
        this.findTopParent(filteredList[i], topParents);
      }

      for (let i = 0; i < topParents.length; i++) {
        this.addChilds(topParents[i], searchedFoldersList);
      }
    } else {
      searchedFoldersList = this.foldersList.slice();
    }

    // if (this.isfoldersListSortDirectionDesc) {
    //   posList.sort((a, b) => (a.name > b.name ? 1 : b.name > a.name ? -1 : 0));
    // } else {
    //   posList.sort((a, b) => (b.name > a.name ? 1 : a.name > b.name ? -1 : 0));
    // }

    this.sortedFolders = searchedFoldersList;
  }

  addChilds(item: IFolder, searchedFoldersList: IFolder[]) {
    searchedFoldersList.push(item);
    const childs = this.foldersList.filter((x) => x.parentId === item.id);

    for (let i = 0; i < childs.length; i++) {
      this.addChilds(childs[i], searchedFoldersList);
    }
  }

  findTopParent(item: IFolder, topParents: IFolder[]) {
    if (item.parentId > 0) {
      const parent = this.foldersList.find((x) => x.id === item.parentId);
      if (parent) {
        this.findTopParent(parent, topParents);
      }
    } else {
      const isNotExist =
        topParents.find((x) => x.id === item?.id) === undefined;
      if (isNotExist) {
        topParents.push(item);
      }
    }
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
      this.sortedFolders = [...res.data];
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
      if (this.selectedFolder && this.selectedFolder.id === folderId) {
        await this.rootStore.candsStore.getFolderCandsList();
      }

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
      if (this.selectedFolder && this.selectedFolder.id === folderId) {
        await this.rootStore.candsStore.getFolderCandsList();
      }

      this.rootStore.candsStore.updateCandListFolderAttached(res.data);
    }

    this.rootStore.generalStore.backdrop = false;
  }
}
