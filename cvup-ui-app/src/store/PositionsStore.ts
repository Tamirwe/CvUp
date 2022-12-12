import { makeAutoObservable } from "mobx";
import { IPosition } from "../models/AuthModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;

  constructor(private rootStore: RootStore) {
    makeAutoObservable(this);
    this.positionApi = new PositionsApi();
  }

  async addUpdatePosition(position: IPosition) {
    return await this.positionApi.addUpdatePosition(position);
  }
}
