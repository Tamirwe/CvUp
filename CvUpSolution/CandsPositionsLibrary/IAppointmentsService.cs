using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IAppointmentsService
    {
        Task<int> AddAppointment(AppointmentModel appointment, UserModel? user);
    }
}
