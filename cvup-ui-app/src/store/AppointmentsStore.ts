import { makeAutoObservable } from "mobx";
import { IAppointmentSend, IAppSettings } from "../models/GeneralModels";
import AppointmentsApi from "./api/AppointmentsApi";
import { RootStore } from "./RootStore";

export class AppointmentsStore {
  private appointmentsApi;

  constructor(private rootStore: RootStore, appSettings: IAppSettings) {
    makeAutoObservable(this);
    this.appointmentsApi = new AppointmentsApi(appSettings);
  }

  reset() {}

  async getAppointment(positionId?: number, candidateId?: number) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.appointmentsApi.getAppointment(
      positionId,
      candidateId
    );

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async addAppointment(appointment: IAppointmentSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.appointmentsApi.addAppointment(appointment);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async updateAppointment(appointment: IAppointmentSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.appointmentsApi.updateAppointment(appointment);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }

  async cancelAppointment(appointment: IAppointmentSend) {
    this.rootStore.generalStore.backdrop = true;

    const res = await this.appointmentsApi.cancelAppointment(appointment);

    this.rootStore.generalStore.backdrop = false;

    return res;
  }
}
