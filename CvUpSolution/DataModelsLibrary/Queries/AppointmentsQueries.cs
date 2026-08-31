using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataModelsLibrary.Queries
{
    public class AppointmentsQueries : IAppointmentsQueries
    {
        public AppointmentsQueries()
        {
        }

        /// <summary>
        /// The appointment of a candidate on a position, as long as it did not happen yet.
        /// An interview that is over starts a new appointment.
        /// </summary>
        public async Task<AppointmentModel?> GetAppointment(int companyId, int? positionId, int? candidateId)
        {
            using var dbContext = new cvupdbContext();

            var query = dbContext.appointments
                .AsNoTracking()
                .Where(x => x.company_id == companyId && x.start_time >= DateTime.UtcNow);

            query = positionId.HasValue
                ? query.Where(x => x.position_id == positionId)
                : query.Where(x => x.position_id == null);

            query = candidateId.HasValue
                ? query.Where(x => x.candidate_id == candidateId)
                : query.Where(x => x.candidate_id == null);

            var row = await query.OrderBy(x => x.start_time).FirstOrDefaultAsync();

            if (row == null)
            {
                return null;
            }

            return new AppointmentModel
            {
                id = row.id,
                companyId = row.company_id,
                userId = row.user_id,
                positionId = row.position_id,
                candidateId = row.candidate_id,
                subject = row.subject,
                start = new DateTimeOffset(DateTime.SpecifyKind(row.start_time, DateTimeKind.Utc)),
                durationMinutes = row.duration_minutes,
                location = row.location,
                notes = row.notes,
                candidates = Guests(row.candidates),
                contacts = Guests(row.contacts),
                optionalParticipants = Guests(row.optional_participants),
            };
        }

        private List<EmailAddress> Guests(string? guestsJson)
        {
            if (string.IsNullOrWhiteSpace(guestsJson))
            {
                return new List<EmailAddress>();
            }

            return JsonConvert.DeserializeObject<List<EmailAddress>>(guestsJson) ?? new List<EmailAddress>();
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

        public async Task<(string icsUid, int sequence)> GetIcsInfo(int appointmentId)
        {
            using var dbContext = new cvupdbContext();

            var icsInfo = await dbContext.appointments
                .AsNoTracking()
                .Where(x => x.id == appointmentId)
                .Select(x => new { x.ics_uid, x.ics_sequence })
                .FirstAsync();

            return (icsInfo.ics_uid, icsInfo.ics_sequence);
        }

        /// <summary>
        /// The ics uid is never touched, an update has to keep pointing at the same event.
        /// </summary>
        public async Task UpdateAppointment(AppointmentModel appointment, int sequence)
        {
            using var dbContext = new cvupdbContext();

            var editAppointment = await dbContext.appointments
                .FirstAsync(x => x.id == appointment.id && x.company_id == appointment.companyId);

            editAppointment.position_id = appointment.positionId;
            editAppointment.candidate_id = appointment.candidateId;
            editAppointment.subject = appointment.subject;
            editAppointment.start_time = appointment.start.UtcDateTime;
            editAppointment.duration_minutes = appointment.durationMinutes;
            editAppointment.location = appointment.location;
            editAppointment.notes = appointment.notes;
            editAppointment.candidates = JsonConvert.SerializeObject(appointment.candidates);
            editAppointment.contacts = JsonConvert.SerializeObject(appointment.contacts);
            editAppointment.optional_participants = JsonConvert.SerializeObject(appointment.optionalParticipants);
            editAppointment.ics_sequence = sequence;
            editAppointment.updated_at = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// The appointment is removed, the cancellation itself is sent out of the
        /// model the caller already holds.
        /// </summary>
        public async Task CancelAppointment(int appointmentId)
        {
            using var dbContext = new cvupdbContext();

            var deleteAppointment = await dbContext.appointments.FirstOrDefaultAsync(x => x.id == appointmentId);

            if (deleteAppointment == null)
            {
                return;
            }

            dbContext.appointments.Remove(deleteAppointment);
            await dbContext.SaveChangesAsync();
        }
    }
}
