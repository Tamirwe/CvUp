import { makeAutoObservable, runInAction } from "mobx";
import { IAppSettings } from "../models/GeneralModels";
import { Iohlc } from "../models/FuStatModel";
import FuturesStatisticsApi from "./api/FuturesStatisticsApi";
import { RootStore } from "./RootStore";

export class FuturesStatisticStore {
  private futuresStatisticsApi;

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
      this.dailyStatList = this.calculateOverlaping(res.data);
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

  calculateAveragesMedians() {
    if (!this.dailyStatList || this.dailyStatList.length === 0) {
      return null;
    }

    let sumPoints = 0;
    let sumOverlapPercent = 0;
    const dayPointsArr: number[] = [];
    const dayOverlapPercent: number[] = [];

    this.dailyStatList.forEach((item) => {
      dayPointsArr.push(item.dayPoints!);
      sumPoints += item.dayPoints!;

      if (item.overlapPercent) {
        dayOverlapPercent.push(item.overlapPercent);
        sumOverlapPercent += item.overlapPercent;
      }
    });

    const pointsAverage = this.calculateAverage(sumPoints, dayPointsArr.length);
    const pointsMedian = this.calculateMedian(dayPointsArr);

    console.log(
      "pointsAverage: " + pointsAverage + "    pointsMedian: " + pointsMedian
    );

    const overlapAverage = this.calculateAverage(
      sumOverlapPercent,
      dayOverlapPercent.length
    );
    const overlapMedian = this.calculateMedian(dayOverlapPercent);

    console.log(
      "overlapAverage: " +
        overlapAverage +
        "    overlapMedian: " +
        overlapMedian
    );

    // const average = Math.round(sumPoints / this.dailyStatList.length);
    // const sortedArr = [...dayPointsArr].sort((a, b) => a - b);
    // const length = sortedArr.length;
    // const midpoint = Math.floor(length / 2);
    // const median = sortedArr[midpoint];

    // return { average: average, median: median };
    // return { averageDayPoints: averageDayPoints, medianDayPoints: medianDayPoints };
  }

  calculateAverage(sumData: number, dataLength: number) {
    const average = Math.round(sumData / dataLength);
    return average;
  }

  calculateMedian(numberArray: number[]) {
    const sortedArr = numberArray.sort((a, b) => a - b);
    const length = numberArray.length;
    const midpoint = Math.floor(length / 2);
    return sortedArr[midpoint];
  }

  calculateOverlaping(ohlcLis: Iohlc[]) {
    const statListSorted = [...ohlcLis];
    let overlapHigh = 0;
    let overlaplow = 0;

    for (let i = 0; i < statListSorted.length - 1; i++) {
      overlapHigh = statListSorted[i].high!;
      if (overlapHigh > statListSorted[i + 1].high!) {
        overlapHigh = statListSorted[i + 1].high!;
      }

      statListSorted[i].highOverlap = overlapHigh;

      overlaplow = statListSorted[i].low!;
      if (overlaplow < statListSorted[i + 1].low!) {
        overlaplow = statListSorted[i + 1].low!;
      }

      statListSorted[i].lowOverlap = overlaplow;
      statListSorted[i].overlapPoints = Math.abs(overlapHigh - overlaplow);
      statListSorted[i].overlapPercent = Math.round(
        (statListSorted[i].overlapPoints! / statListSorted[i + 1].dayPoints!) *
          100
      );
    }

    return [...statListSorted];
  }
}
