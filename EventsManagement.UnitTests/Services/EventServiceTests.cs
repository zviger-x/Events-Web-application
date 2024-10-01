﻿using AutoMapper;
using EventsManagement.BusinessLogic.DataTransferObjects;
using EventsManagement.BusinessLogic.Services.EventService;
using EventsManagement.BusinessLogic.UnitOfWork;
using EventsManagement.BusinessLogic.Validation.Validators;
using EventsManagement.DataAccess.Repositories.Interfaces;
using EventsManagement.DataObjects.Entities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using MockQueryable;
using FluentValidation;

namespace EventsManagement.UnitTests.Services
{
    [TestFixture]
    internal class EventServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEventRepository> _eventRepositoryMock;
        private EventValidator _validator;

        private EventCreateUseCase _eventCreateUseCase;
        private EventUpdateUseCase _eventUpdateUseCase;
        private EventDeleteUseCase _eventDeleteUseCase;
        private EventGetAllUseCase _eventGetAllUseCase;
        private EventGetByCategoryUseCase _eventGetByCategoryUseCase;
        private EventGetByDateUseCase _eventGetByDateUseCase;
        private EventGetByIdUseCase _eventGetByIdUseCase;
        private EventGetByNameUseCase _eventGetByNameUseCase;
        private EventGetByVenueUseCase _eventGetByVenueUseCase;

        [SetUp]
        public void Setup()
        {
            var eventsMock = new List<Event>().AsQueryable().BuildMock();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _eventRepositoryMock.Setup(repo => repo.GetAll()).Returns(eventsMock);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uow => uow.EventRepository).Returns(_eventRepositoryMock.Object);
            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(m => m.ConfigurationProvider).Returns(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Event, EventDTO>();
                cfg.CreateMap<EventDTO, Event>();
            }));

            _validator = new EventValidator(_unitOfWorkMock.Object);
            _eventCreateUseCase = new EventCreateUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventUpdateUseCase = new EventUpdateUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventDeleteUseCase = new EventDeleteUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetAllUseCase = new EventGetAllUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetByCategoryUseCase = new EventGetByCategoryUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetByDateUseCase = new EventGetByDateUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetByIdUseCase = new EventGetByIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetByNameUseCase = new EventGetByNameUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
            _eventGetByVenueUseCase = new EventGetByVenueUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _validator);
        }

        #region --- Positive Cases ---
        [Test]
        public async Task EventCreateUseCase_ShouldCreateEvent()
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                Category = "Category",
                DateAndTime = DateTime.Now,
                MaxNumberOfParticipants = 1,
                Venue = "Venue"
            };
            var eventEntity = _mapperMock.Object.Map<Event>(eventDTO);

            _mapperMock.Setup(m => m.Map<Event>(eventDTO)).Returns(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.CreateAsync(eventEntity)).Returns(Task.CompletedTask);

            // Act
            await _eventCreateUseCase.CreateAsync(eventDTO);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.CreateAsync(eventEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EventUpdateUseCase_ShouldUpdateEvent()
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                Category = "Category",
                DateAndTime = DateTime.Now,
                MaxNumberOfParticipants = 1,
                Venue = "Venue"
            };
            var eventEntity = _mapperMock.Object.Map<Event>(eventDTO);

            _mapperMock.Setup(m => m.Map<Event>(eventDTO)).Returns(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.Update(eventEntity)).Verifiable();

            // Act
            await _eventUpdateUseCase.UpdateAsync(eventDTO);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.Update(eventEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EventDeleteUseCase_ShouldDeleteEvent()
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                Category = "Category",
                DateAndTime = DateTime.Now,
                MaxNumberOfParticipants = 1,
                Venue = "Venue"
            };
            var eventEntity = _mapperMock.Object.Map<Event>(eventDTO);

            _mapperMock.Setup(m => m.Map<Event>(eventDTO)).Returns(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.Delete(eventEntity)).Verifiable();

            // Act
            await _eventDeleteUseCase.DeleteAsync(eventDTO);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.Delete(eventEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void EventGetAllUseCase_ShouldReturnAllEvents()
        {
            // Arrange
            var eventEntities = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" }
            }.AsQueryable();

            var eventDTOs = new List<EventDTO>
            {
                new EventDTO { Id = 1, Name = "Event 1" },
                new EventDTO { Id = 2, Name = "Event 2" }
            }.AsQueryable();

            _mapperMock.Setup(m => m.ProjectTo<EventDTO>(eventEntities, null)).Returns(eventDTOs);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetAll()).Returns(eventEntities);

            // Act
            var result = _eventGetAllUseCase.GetAll();

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetAll(), Times.Once);
            ClassicAssert.AreEqual(eventDTOs.Count(), result.Count());
        }

        [Test]
        public void EventGetByCategoryUseCase_ShouldReturnEventsByCategory()
        {
            // Arrange
            var category = "Category";
            var eventEntities = new List<Event>
            {
                new Event { Id = 1, Name = "Name 1", Category = category },
                new Event { Id = 2, Name = "Name 2", Category = category }
            }.AsQueryable();

            var eventDTOs = new List<EventDTO>
            {
                new EventDTO { Id = 1, Name = "Name 1", Category = category },
                new EventDTO { Id = 2, Name = "Name 2", Category = category }
            }.AsQueryable();

            _mapperMock.Setup(m => m.ProjectTo<EventDTO>(eventEntities, null)).Returns(eventDTOs);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByCategory(category)).Returns(eventEntities);

            // Act
            var result = _eventGetByCategoryUseCase.GetByCategory(category);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetByCategory(category), Times.Once);
            ClassicAssert.AreEqual(eventDTOs.Count(), result.Count());
        }

        [Test]
        public void EventGetByDateUseCase_ShouldReturnEventsByDate()
        {
            // Arrange
            var date = new DateTime(2024, 09, 28);
            var eventEntities = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", DateAndTime = date },
                new Event { Id = 2, Name = "Event 2", DateAndTime = date }
            }.AsQueryable();

            var eventDTOs = new List<EventDTO>
            {
                new EventDTO { Id = 1, Name = "Event 1", DateAndTime = date },
                new EventDTO { Id = 2, Name = "Event 2", DateAndTime = date }
            }.AsQueryable();

            _mapperMock.Setup(m => m.ProjectTo<EventDTO>(eventEntities, null)).Returns(eventDTOs);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByDate(date)).Returns(eventEntities);

            // Act
            var result = _eventGetByDateUseCase.GetByDate(date);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetByDate(date), Times.Once);
            ClassicAssert.AreEqual(eventDTOs.Count(), result.Count());
        }

        [Test]
        public async Task EventGetByIdUseCase_ShouldReturnEventById()
        {
            // Arrange
            var eventId = 1;
            var eventEntity = new Event { Id = eventId, Name = "Event 1" };
            var eventDto = new EventDTO { Id = eventId, Name = "Event 2" };

            _mapperMock.Setup(m => m.Map<EventDTO>(eventEntity)).Returns(eventDto);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);

            // Act
            var result = await _eventGetByIdUseCase.GetByIdAsync(eventId);

            // Assert
            _mapperMock.Verify(m => m.Map<EventDTO>(eventEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetByIdAsync(eventId), Times.Once);
            ClassicAssert.AreEqual(eventDto, result);
        }

        [Test]
        public async Task EventGetByNameUseCase_ShouldReturnEventsByName()
        {
            // Arrange
            var eventName = "Name";
            var eventEntity = new Event { Id = 1, Name = eventName };
            var eventDto = new EventDTO { Id = 1, Name = eventName };

            _mapperMock.Setup(m => m.Map<EventDTO>(eventEntity)).Returns(eventDto);
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByNameAsync(eventName)).ReturnsAsync(eventEntity);

            // Act
            var result = await _eventGetByNameUseCase.GetByNameAsync(eventName);

            // Assert
            _mapperMock.Verify(m => m.Map<EventDTO>(eventEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetByNameAsync(eventName), Times.Once);
            ClassicAssert.AreEqual(eventDto, result);
        }

        [Test]
        public void EventGetByVenueUseCase_ShouldReturnEventsByVenue()
        {
            // Arrange
            var venue = "Sample Venue";
            var eventEntities = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Venue = venue },
                new Event { Id = 2, Name = "Event 2", Venue = venue }
            }.AsQueryable();

            var eventDTOs = new List<EventDTO>
            {
                new EventDTO { Id = 1, Name = "Event 1", Venue = venue },
                new EventDTO { Id = 2, Name = "Event 2", Venue = venue }
            }.AsQueryable();

            _mapperMock.Setup(m => m.ProjectTo<EventDTO>(eventEntities, null)).Returns(eventDTOs.AsQueryable());
            _unitOfWorkMock.Setup(uow => uow.EventRepository.GetByVenue(venue)).Returns(eventEntities);

            // Act
            var result = _eventGetByVenueUseCase.GetByVenue(venue);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.EventRepository.GetByVenue(venue), Times.Once);
            ClassicAssert.AreEqual(eventDTOs.Count(), result.Count());
        }
        #endregion

        #region --- Negative cases ---
        [TestCase(null, "Description", "Category", 10)]
        [TestCase("", "Description", "Category", 10)]
        [TestCase("Name", null, "Category", 10)]
        [TestCase("Name", "", "Category", 10)]
        [TestCase("Name", "Description", null, 10)]
        [TestCase("Name", "Description", "", 10)]
        [TestCase("Name", "Description", "Category", 0)]
        public async Task EventCreateUseCase_ShouldThrowValidationException(string name, string description, string category, int maxParticipants)
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Name = name,
                Description = description,
                Category = category,
                MaxNumberOfParticipants = maxParticipants
            };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _eventCreateUseCase.CreateAsync(eventDTO));
        }

        [TestCase(null, "Description", "Category", 10)]
        [TestCase("", "Description", "Category", 10)]
        [TestCase("Name", null, "Category", 10)]
        [TestCase("Name", "", "Category", 10)]
        [TestCase("Name", "Description", null, 10)]
        [TestCase("Name", "Description", "", 10)]
        [TestCase("Name", "Description", "Category", 0)]
        public async Task EventUpdateUseCase_ShouldThrowValidationException(string name, string description, string category, int maxParticipants)
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Name = name,
                Description = description,
                Category = category,
                MaxNumberOfParticipants = maxParticipants
            };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _eventUpdateUseCase.UpdateAsync(eventDTO));
        }

        [TestCase(null, "Description", "Category", 10)]
        [TestCase("", "Description", "Category", 10)]
        [TestCase("Name", null, "Category", 10)]
        [TestCase("Name", "", "Category", 10)]
        [TestCase("Name", "Description", null, 10)]
        [TestCase("Name", "Description", "", 10)]
        [TestCase("Name", "Description", "Category", 0)]
        public async Task EventDeleteUseCase_ShouldThrowValidationException(string name, string description, string category, int maxParticipants)
        {
            // Arrange
            var eventDTO = new EventDTO
            {
                Name = name,
                Description = description,
                Category = category,
                MaxNumberOfParticipants = maxParticipants
            };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _eventDeleteUseCase.DeleteAsync(eventDTO));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EventGetByCategoryUseCase_ShouldThrowArgumentNullException(string category)
        {
            Assert.Throws<ArgumentNullException>(() => _eventGetByCategoryUseCase.GetByCategory(category));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task EventGetByNameUseCase_ShouldThrowArgumentNullException(string name)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _eventGetByNameUseCase.GetByNameAsync(name));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EventGetByVenueUseCase_ShouldThrowArgumentNullException(string venue)
        {
            Assert.Throws<ArgumentNullException>(() => _eventGetByVenueUseCase.GetByVenue(venue));
        }

        [TestCase(-1)]
        [TestCase(-512)]
        [TestCase(int.MinValue)]
        public async Task EventGetByIdUseCase_ShouldThrowArgumentOutOfRangeException(int id)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _eventGetByIdUseCase.GetByIdAsync(id));
        }
        #endregion
    }
}