﻿using EventsManagement.BusinessLogic.DataTransferObjects;
using EventsManagement.BusinessLogic.UseCases.Interfaces;

namespace EventsManagement.BusinessLogic.UseCases.Interfaces.Event
{
    public interface IGetEventsSortedAndPaginatedUseCase 
        : IUseCase<(string? sortBy, string? value, string? pageNumber, int pageSize), Task<IEnumerable<EventDTO>>>
    {
    }
}
