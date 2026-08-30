using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAppointmentsQueries
    {
        Task<int> AddAppointment(AppointmentModel appointment, string icsUid);
    }
}
