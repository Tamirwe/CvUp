using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IScheduleInterviewService
    {
        Task<ScheduleInterviewModel?> GetScheduleInterview(int companyId, int? positionId, int? candidateId);
        Task<int> AddScheduleInterview(ScheduleInterviewModel scheduleInterview, UserModel? user);
        Task UpdateScheduleInterview(ScheduleInterviewModel scheduleInterview, UserModel? user);
        Task CancelScheduleInterview(ScheduleInterviewModel scheduleInterview, UserModel? user);
    }
}
