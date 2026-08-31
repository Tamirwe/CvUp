using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IAppointmentsService
    {
        Task<AppointmentModel?> GetAppointment(int companyId, int? positionId, int? candidateId);
        Task<int> AddAppointment(AppointmentModel appointment, UserModel? user);
        Task UpdateAppointment(AppointmentModel appointment, UserModel? user);
        Task CancelAppointment(AppointmentModel appointment, UserModel? user);
    }
}
