using System.Text.Json.Serialization;

namespace RaceDay.MVC.ViewModels
{
    public class EnrolmentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("participantName")]
        public string ParticipantName { get; set; } = string.Empty;
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ResultDto
    {
        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;
        [JsonPropertyName("eventDate")]
        public DateTime EventDate { get; set; }
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = string.Empty;
        [JsonPropertyName("finishTime")]
        public TimeSpan FinishTime { get; set; }
        [JsonPropertyName("position")]
        public int Position { get; set; }
    }
}