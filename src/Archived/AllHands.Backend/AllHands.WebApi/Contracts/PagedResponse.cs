using AllHands.Application.Dto;

namespace AllHands.WebApi.Contracts;

public record PagedResponse<TResponse>(IReadOnlyList<TResponse> Data, int TotalCount)
{
}

public static class PagedResponse
{
    public static PagedResponse<TDto> FromDto<TDto>(PagedDto<TDto> dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        
        return new PagedResponse<TDto>(dto.Data, dto.TotalCount);
    }
    
    public static PagedResponse<TResponse> FromDto<TDto, TResponse>(PagedDto<TDto> dto, Func<TDto, TResponse> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(dto);
        
        return new PagedResponse<TResponse>(dto.Data.Select(map).ToList(), dto.TotalCount);
    }
}