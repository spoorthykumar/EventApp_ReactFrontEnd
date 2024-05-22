import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

const App = () => {
  const [venues, setVenues] = useState([]);
  const [selectedVenue, setSelectedVenue] = useState('');
  const [events, setEvents] = useState([]);
  const [selectedEvent, setSelectedEvent] = useState(null);

  // Fetch venues on component mount
  useEffect(() => {
    const fetchVenues = async () => {
      try {
        const response = await axios.get('https://eventlistwebapp1.azurewebsites.net/Events/venues');
        console.log('Fetched venues:', response.data);
        setVenues(response.data);
      } catch (error) {
        console.error("Error fetching venues", error);
      }
    };

    fetchVenues();
  }, []);

  // Fetch events when selectedVenue changes
  useEffect(() => {
    const fetchEvents = async () => {
      try {
        console.log('Fetching events for venue:', selectedVenue);
        const response = await axios.get('https://eventlistwebapp1.azurewebsites.net/Events', {
          params: { venue: selectedVenue }
        });
        console.log('Fetched events:', response.data);
        setEvents(response.data);
      } catch (error) {
        console.error("Error fetching events", error);
      }
    };

    if (selectedVenue) {
      fetchEvents();
    } else {
      // Fetch all events if no venue is selected
      const fetchAllEvents = async () => {
        try {
          const response = await axios.get('https://eventlistwebapp1.azurewebsites.net/Events');
          console.log('Fetched all events:', response.data);
          setEvents(response.data);
        } catch (error) {
          console.error("Error fetching all events", error);
        }
      };
      fetchAllEvents();
    }
  }, [selectedVenue]);

  // Fetch event details when an event is clicked
  const fetchEventDetails = async (id) => {
    console.log('Fetching details for event ID:', id);
    try {
      const response = await axios.get(`https://eventlistwebapp1.azurewebsites.net/Events/${id}`);
      console.log('Fetched event details:', response.data);
      setSelectedEvent(response.data);
    } catch (error) {
      console.error("Error fetching event details", error);
    }
  };

  return (
    <div className="App">
      <h1>Event Listing</h1>
      <label htmlFor="venue-select">Select Venue: </label>
      <select
        id="venue-select"
        value={selectedVenue}
        onChange={(e) => setSelectedVenue(e.target.value)}
      >
        <option value="">All Venues</option>
        {venues.map((venue) => (
          <option key={venue.id} value={venue.name}>{venue.name}</option>
        ))}
      </select>
      {selectedEvent && (
        <div className="event-details">
          <h2>{selectedEvent.name}</h2>
          <p>Date: {new Date(selectedEvent.startDate).toLocaleString()}</p>
          <p>Venue: {selectedEvent.venue}</p>
          <p>Location: {selectedEvent.location}</p>
          <p>Capacity: {selectedEvent.capacity}</p>
          <p>Description: {selectedEvent.description}</p>
        </div>
      )}
      <div className="events">
        {events.map(event => (
          <div key={event.id} className="event" onClick={() => fetchEventDetails(event.id)}>
            <h2>{event.name}</h2>
            <p>{new Date(event.startDate).toLocaleString()}</p>
            <p>Venue: {event.venue}</p>
          </div>
        ))}
      </div>      
    </div>
  );
};

export default App;
