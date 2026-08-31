using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAppointmentsQueries
    {
        Task<AppointmentModel?> GetAppointment(int companyId, int? positionId, int? candidateId);
        Task<int> AddAppointment(AppointmentModel appointment, string icsUid);
        Task<(string icsUid, int sequence)> GetIcsInfo(int appointmentId);
        Task UpdateAppointment(AppointmentModel appointment, int sequence);
        Task CancelAppointment(int appointmentId);
    }
}
