using System.Text.Json.Serialization;

namespace EventApp.Models
{
    public class Venue
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
}
