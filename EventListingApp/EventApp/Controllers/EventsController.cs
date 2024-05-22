using Microsoft.AspNetCore.Mvc;
using EventApp.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EventApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EventsController> _logger;
        private const string EventDataUrl = "https://teg-coding-challenge.s3.ap-southeast-2.amazonaws.com/events/event-data.json";

        public EventsController(HttpClient httpClient, ILogger<EventsController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("venues")]
        public async Task<IActionResult> GetVenues()
        {
            try
            {
                var responseString = await _httpClient.GetStringAsync(EventDataUrl);
                var eventData = JsonSerializer.Deserialize<EventData>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventData == null || eventData.Venues == null)
                {
                    return NoContent();
                }

                return Ok(eventData.Venues);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error deserializing JSON data");
                return StatusCode(500, "Error processing data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching venues");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string? venue)
        {
            try
            {
                var responseString = await _httpClient.GetStringAsync(EventDataUrl);
                var eventData = JsonSerializer.Deserialize<EventData>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventData == null || eventData.Events == null || eventData.Venues == null)
                {
                    return NoContent();
                }

                if (!string.IsNullOrEmpty(venue))
                {
                    var venueData = eventData.Venues.FirstOrDefault(v => v.Name.Equals(venue, StringComparison.OrdinalIgnoreCase));
                    if (venueData != null)
                    {
                        eventData.Events = eventData.Events.Where(e => e.VenueId == venueData.Id).ToList();
                    }
                    else
                    {
                        eventData.Events = new List<Event>();
                    }
                }

                var eventDetails = eventData.Events.Join(
                    eventData.Venues,
                    e => e.VenueId,
                    v => v.Id,
                    (e, v) => new
                    {
                        e.Id,
                        e.Name,
                        e.StartDate,
                        Venue = v.Name,
                        v.Location,
                        v.Capacity,
                        e.Description
                    }
                );

                return Ok(eventDetails);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error deserializing JSON data");
                return StatusCode(500, "Error processing data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching events");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var responseString = await _httpClient.GetStringAsync(EventDataUrl);
                var eventData = JsonSerializer.Deserialize<EventData>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventData == null || eventData.Events == null || eventData.Venues == null)
                {
                    return NoContent();
                }

                var eventItem = eventData.Events.FirstOrDefault(e => e.Id == id);
                if (eventItem == null)
                {
                    return NotFound();
                }

                var venueItem = eventData.Venues.FirstOrDefault(v => v.Id == eventItem.VenueId);
                if (venueItem == null)
                {
                    return NotFound();
                }

                var eventDetails = new
                {
                    eventItem.Id,
                    eventItem.Name,
                    eventItem.StartDate,
                    Venue = venueItem.Name,
                    venueItem.Location,
                    venueItem.Capacity,
                    eventItem.Description // Assuming there's no description in the given JSON, set to null
                };

                return Ok(eventDetails);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error deserializing JSON data");
                return StatusCode(500, "Error processing data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching event details");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
