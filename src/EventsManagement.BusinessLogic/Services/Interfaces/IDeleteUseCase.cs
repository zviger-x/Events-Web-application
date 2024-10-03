﻿using EventsManagement.BusinessLogic.DataTransferObjects.Interfaces;

namespace EventsManagement.BusinessLogic.Services.Interfaces
{
    public interface IDeleteUseCase<T>
        where T : IEntityDTO
    {
        /// <summary>
        /// Removes an entity in the repository.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
        Task DeleteAsync(T entity);
    }
}
