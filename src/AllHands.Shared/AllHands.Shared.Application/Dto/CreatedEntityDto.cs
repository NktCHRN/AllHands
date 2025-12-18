namespace AllHands.Shared.Application.Dto;

public record CreatedEntityDto<TId>(TId Id);

public sealed record CreatedEntityDto(Guid Id) : CreatedEntityDto<Guid>(Id);
