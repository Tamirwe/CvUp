import { IAppointmentSend } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class AppointmentsApi extends BaseApi {
  async getAppointment(positionId?: number, candidateId?: number) {
    return await this.apiWrapper2<IAppointmentSend | null>(async () => {
      return await this.http.get("Appointments/GetAppointment", {
        params: { positionId, candidateId },
      });
    });
  }

  async addAppointment(appointment: IAppointmentSend) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.post("Appointments/AddAppointment", appointment);
    });
  }

  async updateAppointment(appointment: IAppointmentSend) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("Appointments/UpdateAppointment", appointment);
    });
  }

  async cancelAppointment(appointment: IAppointmentSend) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("Appointments/CancelAppointment", appointment);
    });
  }
}
