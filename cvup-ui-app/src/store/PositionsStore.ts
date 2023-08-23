import { makeAutoObservable, runInAction } from "mobx";
import {
  IAppSettings,
  IPosition,
  IPositionType,
  IPositionTypeCount,
  ISearchModel,
} from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";
import { isMobile } from "react-device-detect";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";

export class PositionsStore {
  private positionApi;
  private positionsList: IPosition[] = [];
  private positionsTypesList: IPositionType[] = [];
  private positionSelected?: IPosition;
  private positionTypeSelected?: IPositionType;
  private positionEdit?: IPosition;
  private searchPhrase?: ISearchModel;
  isSelectedPositionOnTop?: boolean = false;
  candDisplayPosition: IPosition | undefined;
  sortedPosList: IPosition[] = [];
  sortedPosTypesList: IPositionType[] = [];
  posTypescountList: IPositionTypeCount[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi(appSettings);
  }

  reset() {
    this.positionsList = [];
  }

  get selectedPosition() {
    return this.positionSelected;
  }

  set selectedPosition(val: IPosition | undefined) {
    this.positionSelected = val;
  }

  get selectedPositionType() {
    return this.positionTypeSelected;
  }

  set selectedPositionType(val: IPositionType | undefined) {
    this.positionTypeSelected = val;
  }

  get editPosition() {
    return this.positionEdit;
  }

  set editPosition(val: IPosition | undefined) {
    this.positionEdit = val;
  }

  searchSortPositions(dir?: string, searchVals?: ISearchModel) {
    runInAction(() => {
      if (searchVals) {
        this.searchPhrase = searchVals;
      }

      if (this.searchPhrase && this.searchPhrase.value) {
        this.sortedPosList = this.positionsList.filter((x) =>
          this.searchStringPositions(x).includes(
            this.searchPhrase!.value!.toLowerCase()
          )
        );
      } else {
        this.sortedPosList = this.positionsList.slice();
      }

      //positionsList is already sorted desc order
      if (dir) {
        if (dir === "desc") {
          this.sortedPosList.sort(
            (a, b) =>
              new Date(b.updated).getTime() - new Date(a.updated).getTime()
          );
        } else {
          this.sortedPosList.sort(
            (a, b) =>
              new Date(a.updated).getTime() - new Date(b.updated).getTime()
          );
        }
      }
    });
  }

  searchPositionsTypes(dir?: string, searchVals?: ISearchModel) {
    runInAction(() => {
      if (searchVals) {
        this.searchPhrase = searchVals;
      }

      if (this.searchPhrase && this.searchPhrase.value) {
        this.sortedPosTypesList = this.positionsTypesList.filter((x) =>
          x.typeName
            .toLowerCase()
            .includes(this.searchPhrase!.value!.toLowerCase())
        );
      } else {
        this.sortedPosTypesList = this.positionsTypesList.slice();
      }

      //positionsList is already sorted desc order
      if (dir) {
        if (dir === "desc") {
          this.sortedPosTypesList.sort(
            (a, b) =>
              new Date(b.dateUpdated).getTime() -
              new Date(a.dateUpdated).getTime()
          );
        } else {
          this.sortedPosTypesList.sort(
            (a, b) =>
              new Date(a.dateUpdated).getTime() -
              new Date(b.dateUpdated).getTime()
          );
        }
      }
    });
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
    this.positionSelected = this.positionsList.find((x) => x.id === posId);

    if (posId) {
      await this.getPositionContacts(posId);
      await this.rootStore.candsStore.getPositionCandsList(posId);
    }

    runInAction(() => {
      this.rootStore.candsStore.currentTabCandsLists =
        TabsCandsEnum.PositionCands;

      if (isMobile) {
        this.rootStore.generalStore.leftDrawerOpen = false;
        this.rootStore.generalStore.rightDrawerOpen = true;
      }

      this.isSelectedPositionOnTop = isPositionOnTop;

      if (isPositionOnTop) {
        const posList = [...this.sortedPosList];

        const objIndex = posList.findIndex(
          (x) => x.id === this.selectedPosition?.id
        );

        if (objIndex > -1) {
          posList.splice(objIndex, 1);
        }

        const posObj = this.positionsList.find(
          (x) => x.id === this.selectedPosition?.id
        );

        const clonedPos = Object.assign({}, posObj);

        if (clonedPos) {
          posList.splice(0, 0, clonedPos);
        }

        this.sortedPosList = posList;
      }
    });
  }

  async positionTypeClick(posTypeId: number) {
    this.positionTypeSelected = this.positionsTypesList.find(
      (x) => x.id === posTypeId
    );
    await this.rootStore.candsStore.getPositionTypeCandsList(posTypeId);

    runInAction(() => {
      this.rootStore.candsStore.currentTabCandsLists =
        TabsCandsEnum.PositionTypeCands;

      if (isMobile) {
        this.rootStore.generalStore.leftDrawerOpen = false;
        this.rootStore.generalStore.rightDrawerOpen = true;
      }
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
      this.sortedPosList = [...res.data];
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async getPositionsTypesList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.positionApi.getPositionsTypesList();
    runInAction(() => {
      this.positionsTypesList = res.data;
      this.sortedPosTypesList = [...res.data];
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
    runInAction(() => {
      this.rootStore.generalStore.backdrop = true;
    });

    const res = await this.positionApi.getPositionContactsIds(posId);

    runInAction(() => {
      if (this.positionSelected) {
        this.positionSelected!.contactsIds = [...res.data];
      }

      this.rootStore.generalStore.backdrop = false;
    });
  }

  async deletePosition(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const posIndex = this.positionsList.findIndex((x) => x.id === id);
    const response = await this.positionApi.deletePosition(id);

    if (response.isSuccess) {
      this.positionsList.splice(posIndex, 1);
      this.rootStore.candsStore.currentTabCandsLists = TabsCandsEnum.AllCands;
      this.positionSelected = undefined;
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

  async getPosTypesCounts() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.positionApi.getPosTypesCounts();
    runInAction(() => {
      this.posTypescountList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }
}
