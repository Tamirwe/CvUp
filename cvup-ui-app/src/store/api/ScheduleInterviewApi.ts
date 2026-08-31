import { IScheduleInterviewSend } from "../../models/GeneralModels";
import BaseApi from "./BaseApi";

export default class ScheduleInterviewApi extends BaseApi {
  async getScheduleInterview(positionId?: number, candidateId?: number) {
    return await this.apiWrapper2<IScheduleInterviewSend | null>(async () => {
      return await this.http.get("ScheduleInterview/GetScheduleInterview", {
        params: { positionId, candidateId },
      });
    });
  }

  async addScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    return await this.apiWrapper2<number>(async () => {
      return await this.http.post("ScheduleInterview/AddScheduleInterview", scheduleInterview);
    });
  }

  async updateScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    return await this.apiWrapper2(async () => {
      return await this.http.put("ScheduleInterview/UpdateScheduleInterview", scheduleInterview);
    });
  }

  async cancelScheduleInterview(scheduleInterview: IScheduleInterviewSend) {
    return await this.apiWrapper2(async () => {
      return await this.http.post("ScheduleInterview/CancelScheduleInterview", scheduleInterview);
    });
  }
}
