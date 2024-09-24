﻿using EventsManagement.DataObjects.Entities.Interfaces;

namespace EventsManagement.DataObjects.Entities
{
    internal class Participant : IEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Email { get; set; }
    }
}
