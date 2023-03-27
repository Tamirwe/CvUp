import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings, IPosition } from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;
  positionsList: IPosition[] = [];
  private positionSelected?: IPosition;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi(appSettings);
  }

  reset() {
    this.positionsList = [];
  }

  async newPosition() {
    runInAction(() => {
      this.selectedPosition = {
        id: 0,
        name: "",
        descr: "",
        updated: new Date(),
        isActive: true,
        customerId: 0,
        hrCompaniesIds: [],
        interviewersIds: [],
        candidates: [],
      } as IPosition;
    });
  }

  get selectedPosition() {
    return this.positionSelected;
  }

  set selectedPosition(val: IPosition | undefined) {
    this.positionSelected = val;
  }

  setPosSelected(posId: number) {
    this.selectedPosition = this.positionsList.find((x) => x.id === posId);
  }

  async getPosition(posId: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.positionApi.getPosition(posId);
    runInAction(() => {
      this.selectedPosition = res.data;
    });

    this.rootStore.generalStore.backdrop = false;
  }

  async addUpdatePosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.addUpdatePosition(position);
    this.rootStore.generalStore.backdrop = false;
    return response;
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
    const response = await this.positionApi.deletePosition(positionId);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
