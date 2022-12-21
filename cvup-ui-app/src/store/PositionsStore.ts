import { makeAutoObservable, runInAction } from "mobx";
import { IPosition, IPositionListItem } from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;
  positionsList: IPositionListItem[] | undefined;
  position: IPosition | undefined;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi();
  }

  async GetPosition(positionId: number) {
    const res = await this.positionApi.GetPosition(positionId);
    runInAction(() => {
      this.position = res.data;
    });
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
