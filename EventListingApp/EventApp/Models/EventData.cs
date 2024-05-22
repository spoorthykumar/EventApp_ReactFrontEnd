using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace EventApp.Models
{
    public class EventData
    {
        [JsonPropertyName("events")]
        public List<Event> Events { get; set; }

        [JsonPropertyName("venues")]
        public List<Venue> Venues { get; set; }
    }
}
