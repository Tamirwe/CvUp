import { makeAutoObservable } from "mobx";
import { IScheduleInterviewSend, IAppSettings } from "../models/GeneralModels";
import ScheduleInterviewApi from "./api/ScheduleInterviewApi";
import { RootStore } from "./RootStore";

export class ScheduleInterviewStore {
  private scheduleInterviewApi;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.scheduleInterviewApi = new ScheduleInterviewApi(appSettings);
  }

  reset() {}

  async getScheduleInterview(positionId?: number, candidateId?: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.scheduleInterviewApi.getScheduleInterview(
      positionId,
      candidateId
    );

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async addScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.scheduleInterviewApi.addScheduleInterview(scheduleInterview);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async updateScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.scheduleInterviewApi.updateScheduleInterview(scheduleInterview);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async cancelScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.scheduleInterviewApi.cancelScheduleInterview(scheduleInterview);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }
}
