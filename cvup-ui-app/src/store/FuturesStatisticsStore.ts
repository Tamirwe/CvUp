import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings } from "../models/GeneralModels";
import { Iohlc } from "../models/FuStatModel";
import FuStatApi from "./api/FuturesStatisticsApi";
import { RootStore } from "./RootStore";

export class FuturesStatisticStore {
  private fuStatApi;

  private statDayInputData?: Iohlc;

  dailyStatList: Iohlc[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.fuStatApi = new FuStatApi(appSettings);
  }

  reset() {}

  async getDailyStatList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.fuStatApi.getDailyStatList();
    runInAction(() => {
      this.dailyStatList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async addDailyStat(contactModel: Iohlc) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.fuStatApi.addDailyStat(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateDailyStat(contactModel: Iohlc) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.fuStatApi.updateDailyStat(contactModel);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteDailyStat(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.fuStatApi.deleteDailyStat(id);
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
