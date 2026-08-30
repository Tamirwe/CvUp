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

        public async Task<int> AddAppointment(AppointmentModel appointment, UserModel? user)
        {
            var icsUid = $"{Guid.NewGuid()}@cvup";
            var appointmentId = await _appointmentsQueries.AddAppointment(appointment, icsUid);

            if (user != null)
            {
                await SendInvite(appointment, user, icsUid);
            }

            return appointmentId;
        }

        private async Task SendInvite(AppointmentModel appointment, UserModel user, string icsUid)
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

            var email = new EmailModel
            {
                To = requiredGuests.Concat(optionalGuests).ToList(),
                Subject = appointment.subject,
                Body = InviteBody(appointment, user),
                From = organizer,
                IcsContent = BuildIcs(appointment, organizer, requiredGuests, optionalGuests, icsUid),
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

        private string InviteBody(AppointmentModel appointment, UserModel user)
        {
            var body = new StringBuilder();

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
            List<EmailAddress> requiredGuests, List<EmailAddress> optionalGuests, string icsUid)
        {
            var startUtc = appointment.start.UtcDateTime;
            var endUtc = startUtc.AddMinutes(appointment.durationMinutes);

            var ics = new StringBuilder();

            ics.Append("BEGIN:VCALENDAR\r\n");
            ics.Append("PRODID:-//CvUp//Appointments//EN\r\n");
            ics.Append("VERSION:2.0\r\n");
            ics.Append("CALSCALE:GREGORIAN\r\n");
            ics.Append("METHOD:REQUEST\r\n");
            ics.Append("BEGIN:VEVENT\r\n");
            ics.Append($"UID:{icsUid}\r\n");
            ics.Append($"DTSTAMP:{IcsDate(DateTime.UtcNow)}\r\n");
            ics.Append($"DTSTART:{IcsDate(startUtc)}\r\n");
            ics.Append($"DTEND:{IcsDate(endUtc)}\r\n");
            ics.Append($"SUMMARY:{IcsText(appointment.subject)}\r\n");

            if (!string.IsNullOrWhiteSpace(appointment.location))
            {
                ics.Append($"LOCATION:{IcsText(appointment.location)}\r\n");
            }

            if (!string.IsNullOrWhiteSpace(appointment.notes))
            {
                ics.Append($"DESCRIPTION:{IcsText(appointment.notes)}\r\n");
            }

            ics.Append($"ORGANIZER;CN={IcsText(organizer.Name)}:mailto:{organizer.Address}\r\n");

            foreach (var guest in requiredGuests)
            {
                ics.Append(IcsAttendee(guest, "REQ-PARTICIPANT"));
            }

            foreach (var guest in optionalGuests)
            {
                ics.Append(IcsAttendee(guest, "OPT-PARTICIPANT"));
            }

            ics.Append("SEQUENCE:0\r\n");
            ics.Append("STATUS:CONFIRMED\r\n");
            ics.Append("TRANSP:OPAQUE\r\n");
            ics.Append("END:VEVENT\r\n");
            ics.Append("END:VCALENDAR\r\n");

            return ics.ToString();
        }

        private string IcsAttendee(EmailAddress guest, string role)
        {
            var name = string.IsNullOrWhiteSpace(guest.Name) ? guest.Address : guest.Name;

            return $"ATTENDEE;CN={IcsText(name)};ROLE={role};PARTSTAT=NEEDS-ACTION;RSVP=TRUE:mailto:{guest.Address}\r\n";
        }

        private string IcsDate(DateTime utcDate)
        {
            return utcDate.ToString("yyyyMMdd'T'HHmmss'Z'");
        }

        private string IcsText(string? txt)
        {
            return (txt ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");
        }
    }
}
