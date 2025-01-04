using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.RestaurantServices.Dtos;

namespace foodie_connect_backend.Modules.RestaurantServices.Mapper;

public static class RestaurantServiceMapper
{
    public static RestaurantServiceResponseDto ToResponseDto(this Service service)
    {
        var formattedName = service.Name.Replace("-", " ");
        return new RestaurantServiceResponseDto()
        {
            Name = formattedName,
            RestaurantId = service.RestaurantId
        };
    }
}