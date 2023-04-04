import { makeAutoObservable, runInAction } from "mobx";
import { PositionStatusEnum } from "../models/GeneralEnums";
import { IAppSettings, IPosition } from "../models/GeneralModels";
import PositionsApi from "./api/PositionsApi";
import { RootStore } from "./RootStore";

export class PositionsStore {
  private positionApi;
  positionsList: IPosition[] = [];
  private positionSelected?: IPosition;
  private positionEdit?: IPosition;

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
        status: PositionStatusEnum.Active,
        customerId: 0,
        hrCompaniesIds: [],
        interviewersIds: [],
        candidates: [],
        contactsIds: [],
      } as IPosition;
    });
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

  setPosSelected(posId: number) {
    this.selectedPosition = this.positionsList.find((x) => x.id === posId);
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

  async deletePosition(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.deletePosition(id);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async activatePosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.activatePosition(position);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async dactivatePosition(position: IPosition) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.positionApi.dactivatePosition(position);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
