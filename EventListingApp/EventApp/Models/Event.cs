using System.Text.Json.Serialization;

namespace EventApp.Models
{
    public class Event
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("venueId")]
        public int VenueId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
