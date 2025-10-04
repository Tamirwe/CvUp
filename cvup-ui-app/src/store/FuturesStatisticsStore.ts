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

  calculateAverageMedian() {
    if (!this.dailyStatList || this.dailyStatList.length === 0) {
      return null;
    }

    let sum = 0;
    const dayPointsArr: number[] = [];

    this.dailyStatList.forEach((item) => {
      dayPointsArr.push(item.dayPoints!);
      sum = sum + item.dayPoints!;
    });

    const average = Math.round(sum / this.dailyStatList.length);

    const sortedArr = [...dayPointsArr].sort((a, b) => a - b);
    const length = sortedArr.length;
    const midpoint = Math.floor(length / 2);
    const median = sortedArr[midpoint];

    return { average: average, median: median };
  }
}
