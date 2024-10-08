﻿using AutoMapper;
using EventsManagement.BusinessLogic.DataTransferObjects;
using EventsManagement.BusinessLogic.UseCases.Interfaces;
using EventsManagement.BusinessLogic.Validation.Messages;
using EventsManagement.BusinessLogic.Validation.Validators.Interfaces;
using EventsManagement.DataAccess.UnitOfWork;

namespace EventsManagement.BusinessLogic.UseCases.EventUserUseCases
{
    internal class EventUserGetByIdUseCase : BaseUseCase<EventUserDTO>, IGetByIdUseCase<EventUserDTO>
    {
        public EventUserGetByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper, IBaseValidator<EventUserDTO> validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<EventUserDTO> Execute(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), StandartValidationMessages.ParameterIsLessThanZero);

            var e = await _unitOfWork.EventUserRepository.GetByIdAsync(id);
            return _mapper.Map<EventUserDTO>(e);
        }
    }
}
