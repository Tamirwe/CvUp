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
  positionsList: IPositionListItem[] | undefined;
  currentPosition: IPosition = this.newPosition;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi();
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
    return await this.positionApi.addUpdatePosition(position);
  }

  async getPositionsList(loadAgain: boolean) {
    if (!this.positionsList || loadAgain) {
      const res = await this.positionApi.getPositionsList();
      runInAction(() => {
        this.positionsList = res.data;
      });
    }
  }

  async deleteHrCompany(positionId: number) {
    return await this.positionApi.deletePosition(positionId);
  }
}
