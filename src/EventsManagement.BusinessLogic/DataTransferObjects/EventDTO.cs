﻿using EventsManagement.BusinessLogic.DataTransferObjects.Interfaces;

namespace EventsManagement.BusinessLogic.DataTransferObjects
{
    public class EventDTO : IEntityDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateAndTime { get; set; }

        public string Venue { get; set; }

        public string Category { get; set; }

        public int CurrentNumberOfParticipants { get; set; }

        public int MaxNumberOfParticipants { get; set; }

        public byte[]? Image { get; set; }

        public bool IsUpdate { get; set; }
    }
}
