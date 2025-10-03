import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings } from "../models/GeneralModels";
import { Iohlc } from "../models/FuStatModel";
import FuturesStatisticsApi from "./api/FuturesStatisticsApi";
import { RootStore } from "./RootStore";

export class FuturesStatisticStore {
  private futuresStatisticsApi;

  private statDayInputData?: Iohlc;

  dailyStatList: Iohlc[] = [];

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.futuresStatisticsApi = new FuturesStatisticsApi(appSettings);
  }

  reset() {}

  async getDayOhlcList() {
    this.rootStore.generalStore.backdrop = true;
    const res = await this.futuresStatisticsApi.getDayOhlcList();
    runInAction(() => {
      this.dailyStatList = res.data;
    });
    this.rootStore.generalStore.backdrop = false;
  }

  async addDayOhlc(ohlcFormData: Iohlc) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.futuresStatisticsApi.addDayOhlc(ohlcFormData);
    this.getDayOhlcList();
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async updateDayOhlc(ohlcFormData: Iohlc) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.futuresStatisticsApi.updateDayOhlc(
      ohlcFormData
    );
    this.getDayOhlcList();
    this.rootStore.generalStore.backdrop = false;
    return response;
  }

  async deleteDayOhlc(id: number) {
    this.rootStore.generalStore.backdrop = true;
    const response = await this.futuresStatisticsApi.deleteDayOhlc(id);
    this.getDayOhlcList();
    this.rootStore.generalStore.backdrop = false;
    return response;
  }
}
