using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IScheduleInterviewQueries
    {
        Task<ScheduleInterviewModel?> GetScheduleInterview(int companyId, int? positionId, int? candidateId);
        Task<int> AddScheduleInterview(ScheduleInterviewModel scheduleInterview, string icsUid);
        Task<(string icsUid, int sequence)> GetIcsInfo(int scheduleInterviewId);
        Task UpdateScheduleInterview(ScheduleInterviewModel scheduleInterview, int sequence);
        Task CancelScheduleInterview(int scheduleInterviewId);
    }
}
