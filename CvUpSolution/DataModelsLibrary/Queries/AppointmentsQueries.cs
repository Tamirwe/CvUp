using Database.models;
using DataModelsLibrary.Models;
using Newtonsoft.Json;

namespace DataModelsLibrary.Queries
{
    public class AppointmentsQueries : IAppointmentsQueries
    {
        public AppointmentsQueries()
        {
        }

        public async Task<int> AddAppointment(AppointmentModel appointment, string icsUid)
        {
            using var dbContext = new cvupdbContext();

            var newAppointment = new appointment
            {
                company_id = appointment.companyId,
                user_id = appointment.userId,
                position_id = appointment.positionId,
                candidate_id = appointment.candidateId,
                subject = appointment.subject,
                start_time = appointment.start.UtcDateTime,
                duration_minutes = appointment.durationMinutes,
                location = appointment.location,
                notes = appointment.notes,
                candidates = JsonConvert.SerializeObject(appointment.candidates),
                contacts = JsonConvert.SerializeObject(appointment.contacts),
                optional_participants = JsonConvert.SerializeObject(appointment.optionalParticipants),
                ics_uid = icsUid,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
            };

            dbContext.appointments.Add(newAppointment);
            await dbContext.SaveChangesAsync();

            return newAppointment.id;
        }
    }
}
