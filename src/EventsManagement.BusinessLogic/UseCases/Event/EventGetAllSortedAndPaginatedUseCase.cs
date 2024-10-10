﻿using AutoMapper;
using EventsManagement.BusinessLogic.DataTransferObjects;
using EventsManagement.BusinessLogic.Services;
using EventsManagement.BusinessLogic.UseCases.Interfaces.Event;
using EventsManagement.BusinessLogic.Validation.Validators.Interfaces;
using EventsManagement.DataAccess.UnitOfWork;
using EventsManagement.DataObjects.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EventsManagement.BusinessLogic.UseCases.Event
{
    internal class EventGetAllSortedAndPaginatedUseCase : BaseUseCase<EventDTO>, IGetEventsSortedAndPaginatedUseCase
    {
        public EventGetAllSortedAndPaginatedUseCase(IUnitOfWork unitOfWork, IMapper mapper, IBaseValidator<EventDTO> validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<IEnumerable<EventDTO>> Execute((string? sortBy, string? value, string? pageNumber, int pageSize) request)
        {
            Console.WriteLine($"[Service] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");

            if (string.IsNullOrEmpty(request.sortBy) || string.IsNullOrEmpty(request.value))
            {
                Console.WriteLine($"[Service condition 1] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                var e = await _unitOfWork.EventRepository.GetAll().ToListAsync();
                var edtos = _mapper.Map<IEnumerable<EventDTO>>(e);
                if (!int.TryParse(request.pageNumber, out int pn) || request.pageSize <= 1)
                    return edtos;

                var pl = await PaginatedList<EventDTO>.CreateAsync(edtos, pn, request.pageSize);
                return pl.Items;
            }
            else
            {
                Console.WriteLine($"[Service condition 2 start] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                IEnumerable<EventDTO> events;
                switch (request.sortBy)
                {
                    case SortValues.Name:
                        Console.WriteLine($"[Service case name] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                        var e = await _unitOfWork.EventRepository.GetByNameAsync(request.value);
                        var el = new List<EventDTO>();
                        if (e != null) el.Add(_mapper.Map<EventDTO>(e));
                        return el;

                    case SortValues.Category:
                        Console.WriteLine($"[Service case category] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                        var ec = await _unitOfWork.EventRepository.GetByCategory(request.value).ToListAsync();
                        events = _mapper.Map<IEnumerable<EventDTO>>(ec);
                        break;

                    case SortValues.Venue:
                        Console.WriteLine($"[Service case venue] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                        var ev = await _unitOfWork.EventRepository.GetByVenue(request.value).ToListAsync();
                        events = _mapper.Map<IEnumerable<EventDTO>>(ev);
                        break;

                    case SortValues.Date:
                        Console.WriteLine($"[Service case date] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                        if (!DateTime.TryParse(request.value, out DateTime date))
                            return new List<EventDTO>();
                        var ed = await _unitOfWork.EventRepository.GetByDate(date).ToListAsync();
                        events = _mapper.Map<IEnumerable<EventDTO>>(ed);
                        break;

                    default:
                        Console.WriteLine($"[Service case default] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                        var ea = await _unitOfWork.EventRepository.GetAll().ToListAsync();
                        var edtos = _mapper.Map<IEnumerable<EventDTO>>(ea);
                        return edtos;
                }

                Console.WriteLine($"[Service condition 2 end] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");

                if (!int.TryParse(request.pageNumber, out int pageNum) || request.pageSize <= 1)
                    return events;

                Console.WriteLine($"[Service condition 2 end paginated] {request.sortBy}, {request.value}, {request.pageNumber}, {request.pageSize}");
                var paginatedEvents = await PaginatedList<EventDTO>.CreateAsync(events, pageNum, request.pageSize);
                return paginatedEvents.Items;
            }
        }

        private static class SortValues
        {
            public const string Name = "name";
            public const string Category = "category";
            public const string Venue = "venue";
            public const string Date = "date";
        }
    }
}
