namespace DataModelsLibrary.Models
{
    public class ScheduleInterviewModel
    {
        public int id { get; set; }
        public int companyId { get; set; }
        public int userId { get; set; }
        public int? positionId { get; set; }
        public int? candidateId { get; set; }
        public string subject { get; set; } = string.Empty;
        public DateTimeOffset start { get; set; }
        public int durationMinutes { get; set; } = 60;
        public string? location { get; set; }
        public string? notes { get; set; }
        public List<EmailAddress> candidates { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> contacts { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> optionalParticipants { get; set; } = new List<EmailAddress>();
        public bool isAddMeAsOptionalGuest { get; set; }
    }
}
