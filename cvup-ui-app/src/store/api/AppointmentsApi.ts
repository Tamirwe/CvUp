import { IAppointmentSend } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class AppointmentsApi extends BaseApi {
  async addAppointment(appointment: IAppointmentSend) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.post("Appointments/AddAppointment", appointment);
    });
  }
}
