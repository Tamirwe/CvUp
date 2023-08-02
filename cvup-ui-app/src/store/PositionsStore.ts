import { makeAutoObservable, runInAction } from "mobx";
import {
  IAppSettings,
  ICand,
  IPosStagesType,
  IPosition,
  ISearchModel,
} from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";
import { isMobile } from "react-device-detect";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";

export class PositionsStore {
  private positionApi;
  private positionsList: IPosition[] = [];
  private positionSelected?: IPosition;
  private positionEdit?: IPosition;
  private searchPhrase?: ISearchModel;
  candDisplayPosition: IPosition | undefined;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi(appSettings);
  }

  reset() {
    this.positionsList = [];
  }

  get positionsSorted() {
    if (this.searchPhrase && this.searchPhrase.value) {
      return this.positionsList.filter((x) =>
        this.searchStringPositions(x).includes(
          this.searchPhrase!.value!.toLowerCase()
        )
      );
    } else {
      return this.positionsList
        .slice()
        .sort(
          (a, b) =>
            new Date(b.updated).getTime() - new Date(a.updated).getTime()
        );
    }
  }

  get selectedPosition() {
    return this.positionSelected;
  }

  set selectedPosition(val: IPosition | undefined) {
    this.positionSelected = val;
  }

  get editPosition() {
    return this.positionEdit;
  }

  set editPosition(val: IPosition | undefined) {
    this.positionEdit = val;
  }

  searchPositions(searchVals: ISearchModel) {
    this.searchPhrase = searchVals;
  }

  searchStringPositions = (x: IPosition) => {
    return (x.name + "").toLowerCase() + (x.customerName + "").toLowerCase();
  };

  updateCandDisplayPosition(candsSource: CandsSourceEnum) {
    this.candDisplayPosition = undefined;

    if (candsSource === CandsSourceEnum.Position) {
      this.candDisplayPosition = Object.assign({}, this.selectedPosition);
    }
  }

  // setPosSelectedById(posId: number, candId?: number) {
  //   this.selectedPosition = this.positionsList.find((x) => x.id === posId);
  // }

  async candDisplayPositionClick(posId: number) {
    await this.positionClick(posId);
    this.updateCandDisplayPosition(CandsSourceEnum.Position);
  }

  async positionClick(posId: number, cand?: ICand) {
    this.selectedPosition = this.positionsList.find((x) => x.id === posId);

    await Promise.all([
      this.rootStore.candsStore.getPositionCands(),
      this.getPositionContacts(posId),
    ]);

    this.rootStore.candsStore.currentTabCandsLists =
      TabsCandsEnum.PositionCands;

    if (isMobile) {
      this.rootStore.generalStore.leftDrawerOpen = false;
      this.rootStore.generalStore.rightDrawerOpen = true;
    }
  }

  async addPosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.addPosition(position);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updatePosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.updatePosition(position);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async getPositionsList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.positionApi.getPositionsList();
    runInAction(() => {
      this.positionsList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getPosition(posId: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.positionApi.getPosition(posId);
    runInAction(() => {
      this.positionEdit = res.data;
    });

    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionContacts(posId: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.positionApi.getPositionContactsIds(posId);

    if (this.positionSelected) {
      this.positionSelected!.contactsIds = res.data;
    }

    this.rootStore.generalStore.backdrop = false;
  }

  async deletePosition(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const posIndex = this.positionsList.findIndex((x) => x.id === id);
    const response = await this.positionApi.deletePosition(id);

    if (response.isSuccess) {
      this.positionsList.splice(posIndex, 1);
      this.rootStore.candsStore.currentTabCandsLists = TabsCandsEnum.AllCands;
      this.selectedPosition = undefined;
    }

    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  findPosName(posId: number) {
    const pos = this.positionsList.find((x) => x.id === posId);
    if (pos) {
      return `${pos?.name} - ${pos?.customerName}`;
    }

    return null;
  }
}
