using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using System.Text;

namespace CandsPositionsLibrary
{
    public class AppointmentsService : IAppointmentsService
    {
        private IAppointmentsQueries _appointmentsQueries;
        private IEmailService _emailService;

        public AppointmentsService(IAppointmentsQueries appointmentsQueries, IEmailService emailService)
        {
            _appointmentsQueries = appointmentsQueries;
            _emailService = emailService;
        }

        public async Task<AppointmentModel?> GetAppointment(int companyId, int? positionId, int? candidateId)
        {
            return await _appointmentsQueries.GetAppointment(companyId, positionId, candidateId);
        }

        public async Task<int> AddAppointment(AppointmentModel appointment, UserModel? user)
        {
            var icsUid = $"{Guid.NewGuid()}@cvup";
            var appointmentId = await _appointmentsQueries.AddAppointment(appointment, icsUid);

            if (user != null)
            {
                await SendInvite(appointment, user, icsUid, 0, "REQUEST");
            }

            return appointmentId;
        }

        /// <summary>
        /// Reschedule / edit. Reuses the original UID so clients update the existing
        /// event in place instead of creating a duplicate.
        /// </summary>
        public async Task UpdateAppointment(AppointmentModel appointment, UserModel? user)
        {
            var (icsUid, sequence) = await _appointmentsQueries.GetIcsInfo(appointment.id);

            sequence++;

            await _appointmentsQueries.UpdateAppointment(appointment, sequence);

            if (user != null)
            {
                await SendInvite(appointment, user, icsUid, sequence, "REQUEST");
            }
        }

        public async Task CancelAppointment(AppointmentModel appointment, UserModel? user)
        {
            var (icsUid, sequence) = await _appointmentsQueries.GetIcsInfo(appointment.id);

            sequence++;

            await _appointmentsQueries.CancelAppointment(appointment.id);

            if (user != null)
            {
                await SendInvite(appointment, user, icsUid, sequence, "CANCEL");
            }
        }

        private async Task SendInvite(AppointmentModel appointment, UserModel user,
            string icsUid, int sequence, string method)
        {
            var organizer = new EmailAddress
            {
                Name = $"{user.firstName} {user.lastName}".Trim(),
                Address = user.email,
            };

            var requiredGuests = DistinctGuests(appointment.candidates.Concat(appointment.contacts));

            var optional = appointment.optionalParticipants.ToList();

            if (appointment.isAddMeAsOptionalGuest)
            {
                optional.Add(organizer);
            }

            // A guest that is already invited as required is not invited again as optional
            var optionalGuests = DistinctGuests(optional)
                .Where(x => !requiredGuests.Any(y => y.Address.Equals(x.Address, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (!requiredGuests.Any() && !optionalGuests.Any())
            {
                return;
            }

            var isCancel = method.Equals("CANCEL", StringComparison.OrdinalIgnoreCase);

            var email = new EmailModel
            {
                To = requiredGuests.Concat(optionalGuests).ToList(),
                Subject = isCancel ? $"בוטל: {appointment.subject}" : appointment.subject,
                Body = InviteBody(appointment, user, isCancel),
                From = organizer,
                IcsContent = BuildIcs(appointment, organizer, requiredGuests, optionalGuests,
                    icsUid, sequence, method),
                IcsMethod = method,
                MailSenderUserName = user.mailUsername,
                MailSenderPassword = user.mailPassword,
            };

            await _emailService.Send(email);
        }

        private List<EmailAddress> DistinctGuests(IEnumerable<EmailAddress> guests)
        {
            return guests
                .Where(x => !string.IsNullOrWhiteSpace(x.Address))
                .GroupBy(x => x.Address.Trim().ToLower())
                .Select(x => x.First())
                .ToList();
        }

        private string InviteBody(AppointmentModel appointment, UserModel user, bool isCancel)
        {
            var body = new StringBuilder();

            if (isCancel)
            {
                body.Append("<p style=\"text-align: right; direction: rtl;\">הפגישה בוטלה.</p>");
            }

            body.Append($"<p style=\"text-align: right; direction: rtl;\">{appointment.subject}</p>");
            body.Append($"<p style=\"text-align: right; direction: rtl;\">{appointment.start.ToLocalTime():dd/MM/yyyy HH:mm} ({appointment.durationMinutes} minutes)</p>");

            if (!string.IsNullOrWhiteSpace(appointment.location))
            {
                body.Append($"<p style=\"text-align: right; direction: rtl;\">{appointment.location}</p>");
            }

            if (!string.IsNullOrWhiteSpace(appointment.notes))
            {
                body.Append($"<p style=\"text-align: right; direction: rtl;\">{appointment.notes}</p>");
            }

            body.Append(user.signature);

            return body.ToString();
        }

        private string BuildIcs(AppointmentModel appointment, EmailAddress organizer,
            List<EmailAddress> requiredGuests, List<EmailAddress> optionalGuests,
            string icsUid, int sequence, string method)
        {
            var isCancel = method.Equals("CANCEL", StringComparison.OrdinalIgnoreCase);

            var startUtc = appointment.start.UtcDateTime;
            var endUtc = startUtc.AddMinutes(appointment.durationMinutes);

            var ics = new StringBuilder();

            void Add(string line) => ics.Append(Fold(line));

            Add("BEGIN:VCALENDAR");
            Add("PRODID:-//CvUp//Appointments//EN");
            Add("VERSION:2.0");
            Add("CALSCALE:GREGORIAN");
            Add($"METHOD:{method}");
            Add("BEGIN:VEVENT");
            Add($"UID:{icsUid}");
            Add($"DTSTAMP:{IcsDate(DateTime.UtcNow)}");
            Add($"DTSTART:{IcsDate(startUtc)}");
            Add($"DTEND:{IcsDate(endUtc)}");
            Add($"SUMMARY:{IcsText(appointment.subject)}");

            if (!string.IsNullOrWhiteSpace(appointment.location))
            {
                Add($"LOCATION:{IcsText(appointment.location)}");
            }

            if (!string.IsNullOrWhiteSpace(appointment.notes))
            {
                Add($"DESCRIPTION:{IcsText(appointment.notes)}");
            }

            Add($"ORGANIZER;CN={IcsParam(organizer.Name)}:mailto:{organizer.Address}");

            // Organizer as CHAIR, already accepted — some clients expect this.
            Add($"ATTENDEE;CN={IcsParam(organizer.Name)};ROLE=CHAIR;PARTSTAT=ACCEPTED;RSVP=FALSE:mailto:{organizer.Address}");

            foreach (var guest in requiredGuests)
            {
                Add(IcsAttendee(guest, "REQ-PARTICIPANT", isCancel));
            }

            foreach (var guest in optionalGuests)
            {
                Add(IcsAttendee(guest, "OPT-PARTICIPANT", isCancel));
            }

            Add($"SEQUENCE:{sequence}");
            Add(isCancel ? "STATUS:CANCELLED" : "STATUS:CONFIRMED");
            Add("TRANSP:OPAQUE");
            Add("END:VEVENT");
            Add("END:VCALENDAR");

            return ics.ToString();
        }

        private string IcsAttendee(EmailAddress guest, string role, bool isCancel)
        {
            var name = string.IsNullOrWhiteSpace(guest.Name) ? guest.Address : guest.Name;

            // A cancellation asks for no response.
            var partstat = isCancel
                ? "PARTSTAT=NEEDS-ACTION;RSVP=FALSE"
                : "PARTSTAT=NEEDS-ACTION;RSVP=TRUE";

            return $"ATTENDEE;CN={IcsParam(name)};ROLE={role};{partstat}:mailto:{guest.Address}";
        }

        private string IcsDate(DateTime utcDate)
        {
            return utcDate.ToString("yyyyMMdd'T'HHmmss'Z'");
        }

        /// <summary>
        /// Escapes a property VALUE (the part after the colon).
        /// </summary>
        private string IcsText(string? txt)
        {
            return (txt ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");
        }

        /// <summary>
        /// Escapes a PARAMETER value (e.g. CN=). Parameters are not backslash-escaped;
        /// values containing ; : or , must be wrapped in double quotes instead.
        /// </summary>
        private string IcsParam(string? txt)
        {
            var value = (txt ?? string.Empty)
                .Replace("\"", "'")
                .Replace("\r", " ")
                .Replace("\n", " ");

            return value.IndexOfAny(new[] { ';', ':', ',' }) >= 0 ? $"\"{value}\"" : value;
        }

        /// <summary>
        /// RFC 5545 line folding. The 75 limit is in OCTETS, not characters, so Hebrew
        /// (2 bytes per char in UTF-8) must be measured by rune to avoid splitting a
        /// character across the fold and corrupting the output.
        /// </summary>
        private static string Fold(string line)
        {
            var sb = new StringBuilder();
            var octets = 0;

            foreach (var rune in line.EnumerateRunes())
            {
                var size = rune.Utf8SequenceLength;

                if (octets + size > 75)
                {
                    sb.Append("\r\n ");
                    octets = 1;
                }

                sb.Append(rune.ToString());
                octets += size;
            }

            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}
