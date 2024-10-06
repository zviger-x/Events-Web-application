import { EventDTO } from "../Models/Events/EventDTO";
import { GetEventsResponse } from "../Models/Events/GetEventsResponse";
import { GetEventByIdResponse } from "../Models/Events/GetEventByIdResponse";
import { API_BASE_URL } from "../../config"
import axios, { AxiosResponse } from "axios";

const APIConnector = {
    GetEvents: async (sortBy?: string, value?: string, page?: string): Promise<EventDTO[]> => {
        const params = new URLSearchParams();
        if (sortBy) params.append('sortby', sortBy);
        if (value) params.append('value', value);
        if (page) params.append('page', page);

        const response: AxiosResponse<GetEventsResponse> = await axios.get(`${API_BASE_URL}/api/events?${params.toString()}`);
        const events = response.data;
        return events;
    },

    CreateEvent: async (event: EventDTO): Promise<void> => {
        await axios.post<number>(`${API_BASE_URL}/api/Events`, event);
    },

    EditEvent: async (event: EventDTO): Promise<void> => {
        await axios.put<number>(`${API_BASE_URL}/api/Events/${event.id}`, event);
    },

    DeleteEvent: async (eventId: number): Promise<void> => {
        await axios.delete<number>(`${API_BASE_URL}/api/Events/${eventId}`);
    },

    GetEventById: async (eventId: number): Promise<EventDTO | undefined> => {
        const response = await axios.get<GetEventByIdResponse>(`${API_BASE_URL}/api/Events/${eventId}`);
        return response.data;
    }
}

export default APIConnector;