import { makeAutoObservable, runInAction } from "mobx";
import { IPosition, IPositionListItem } from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private newPosition: IPosition = {
    id: 0,
    name: "",
    descr: "",
    isActive: true,
    departmentId: 0,
    hrCompaniesIds: [],
    interviewersIds: [],
  };

  private positionApi;
  positionsList: IPositionListItem[] = [];
  currentPosition: IPosition = this.newPosition;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi();
  }

  reset() {
    this.positionsList = [];
    this.currentPosition = this.newPosition;
  }

  async GetPosition(positionId: number) {
    this.rootStore.generalStore.backdrop = true;
    if (positionId === 0) {
      runInAction(() => {
        this.currentPosition = this.newPosition;
      });
    } else {
      const res = await this.positionApi.GetPosition(positionId);
      runInAction(() => {
        this.currentPosition = res.data;
      });
    }
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
