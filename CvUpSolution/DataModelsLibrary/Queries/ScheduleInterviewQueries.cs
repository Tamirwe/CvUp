using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataModelsLibrary.Queries
{
    public class ScheduleInterviewQueries : IScheduleInterviewQueries
    {
        public ScheduleInterviewQueries()
        {
        }

        /// <summary>
        /// The interview of a candidate on a position, as long as it did not happen yet.
        /// An interview that is over starts a new one.
        /// </summary>
        public async Task<ScheduleInterviewModel?> GetScheduleInterview(int companyId, int? positionId, int? candidateId)
        {
            using var dbContext = new cvupdbContext();

            var query = dbContext.schedule_interviews
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

            return new ScheduleInterviewModel
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

        public async Task<int> AddScheduleInterview(ScheduleInterviewModel scheduleInterview, string icsUid)
        {
            using var dbContext = new cvupdbContext();

            var newScheduleInterview = new schedule_interview
            {
                company_id = scheduleInterview.companyId,
                user_id = scheduleInterview.userId,
                position_id = scheduleInterview.positionId,
                candidate_id = scheduleInterview.candidateId,
                subject = scheduleInterview.subject,
                start_time = scheduleInterview.start.UtcDateTime,
                duration_minutes = scheduleInterview.durationMinutes,
                location = scheduleInterview.location,
                notes = scheduleInterview.notes,
                candidates = JsonConvert.SerializeObject(scheduleInterview.candidates),
                contacts = JsonConvert.SerializeObject(scheduleInterview.contacts),
                optional_participants = JsonConvert.SerializeObject(scheduleInterview.optionalParticipants),
                ics_uid = icsUid,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
            };

            dbContext.schedule_interviews.Add(newScheduleInterview);
            await dbContext.SaveChangesAsync();

            return newScheduleInterview.id;
        }

        public async Task<(string icsUid, int sequence)> GetIcsInfo(int scheduleInterviewId)
        {
            using var dbContext = new cvupdbContext();

            var icsInfo = await dbContext.schedule_interviews
                .AsNoTracking()
                .Where(x => x.id == scheduleInterviewId)
                .Select(x => new { x.ics_uid, x.ics_sequence })
                .FirstAsync();

            return (icsInfo.ics_uid, icsInfo.ics_sequence);
        }

        /// <summary>
        /// The ics uid is never touched, an update has to keep pointing at the same event.
        /// </summary>
        public async Task UpdateScheduleInterview(ScheduleInterviewModel scheduleInterview, int sequence)
        {
            using var dbContext = new cvupdbContext();

            var editScheduleInterview = await dbContext.schedule_interviews
                .FirstAsync(x => x.id == scheduleInterview.id && x.company_id == scheduleInterview.companyId);

            editScheduleInterview.position_id = scheduleInterview.positionId;
            editScheduleInterview.candidate_id = scheduleInterview.candidateId;
            editScheduleInterview.subject = scheduleInterview.subject;
            editScheduleInterview.start_time = scheduleInterview.start.UtcDateTime;
            editScheduleInterview.duration_minutes = scheduleInterview.durationMinutes;
            editScheduleInterview.location = scheduleInterview.location;
            editScheduleInterview.notes = scheduleInterview.notes;
            editScheduleInterview.candidates = JsonConvert.SerializeObject(scheduleInterview.candidates);
            editScheduleInterview.contacts = JsonConvert.SerializeObject(scheduleInterview.contacts);
            editScheduleInterview.optional_participants = JsonConvert.SerializeObject(scheduleInterview.optionalParticipants);
            editScheduleInterview.ics_sequence = sequence;
            editScheduleInterview.updated_at = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// The interview is removed, the cancellation itself is sent out of the
        /// model the caller already holds.
        /// </summary>
        public async Task CancelScheduleInterview(int scheduleInterviewId)
        {
            using var dbContext = new cvupdbContext();

            var deleteScheduleInterview = await dbContext.schedule_interviews.FirstOrDefaultAsync(x => x.id == scheduleInterviewId);

            if (deleteScheduleInterview == null)
            {
                return;
            }

            dbContext.schedule_interviews.Remove(deleteScheduleInterview);
            await dbContext.SaveChangesAsync();
        }
    }
}
