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
  private isSelectedPositionOnTop?: boolean = false;
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
      const posList = this.positionsList
        .slice()
        .sort(
          (a, b) =>
            new Date(b.updated).getTime() - new Date(a.updated).getTime()
        );

      if (this.isSelectedPositionOnTop && this.selectedPosition) {
        const objIndex = posList.findIndex(
          (x) => x.id === this.selectedPosition?.id
        );

        if (objIndex > -1) {
          const posArr = posList.splice(objIndex, 1);
          posList.splice(0, 0, posArr[0]);
        }
      }

      return posList;
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

  setRelatedPositionToCandDisplay(
    candsSource: CandsSourceEnum = CandsSourceEnum.Position
  ) {
    this.candDisplayPosition = undefined;

    if (candsSource === CandsSourceEnum.Position) {
      this.candDisplayPosition = Object.assign({}, this.selectedPosition);
    }
  }

  async positionClick(posId: number, isPositionOnTop: boolean = false) {
    return new Promise((resolve) => {
      this.selectedPosition = this.positionsList.find((x) => x.id === posId);

      runInAction(async () => {
        if (posId) {
          await Promise.all([
            this.getPositionContacts(posId),
            this.rootStore.candsStore.getPositionCandsList(posId),
          ]);
        }

        this.rootStore.candsStore.currentTabCandsLists =
          TabsCandsEnum.PositionCands;

        if (isMobile) {
          this.rootStore.generalStore.leftDrawerOpen = false;
          this.rootStore.generalStore.rightDrawerOpen = true;
        }

        this.isSelectedPositionOnTop = isPositionOnTop;
        resolve("");
      });
    });
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
      this.positionSelected!.contactsIds = [...res.data];
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
