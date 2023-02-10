import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, IPosition } from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;
  positionsList: IPosition[] = [];
  posSelected?: IPosition;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi(appSettings);
  }

  reset() {
    this.positionsList = [];
    this.posSelected = undefined;
  }

  async newPosition() {
    runInAction(() => {
      this.posSelected = {
        id: 0,
        name: "",
        descr: "",
        updated: new Date(),
        isActive: true,
        departmentId: 0,
        hrCompaniesIds: [],
        interviewersIds: [],
        candidates: [],
      } as IPosition;
    });
  }

  setPosSelected(posId: number) {
    this.posSelected = this.positionsList.find((x) => x.id === posId);
    this.rootStore.candsStore.currentTabCandsList = "positionCandsList";
  }

  async getPosition(posId: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.positionApi.getPosition(posId);
    runInAction(() => {
      this.posSelected = res.data;
    });

    this.rootStore.generalStore.backdrop = false;
  }

  async addUpdatePosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.positionApi.addUpdatePosition(position);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }

  async getPositionsList(loadAgain: boolean) {
    this.rootStore.generalStore.backdrop = true;
    if (this.positionsList.length === 0 || loadAgain) {
      const res = await this.positionApi.getPositionsList();
      runInAction(() => {
        this.positionsList = res.data;
      });
    }
    this.rootStore.generalStore.backdrop = false;
  }

  async deleteHrCompany(positionId: number) {
    this.rootStore.generalStore.backdrop = true;
    const data = await this.positionApi.deletePosition(positionId);
    this.rootStore.generalStore.backdrop = false;
    return data;
  }
}
