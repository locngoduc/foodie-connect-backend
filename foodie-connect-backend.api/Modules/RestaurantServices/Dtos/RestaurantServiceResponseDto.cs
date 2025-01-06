namespace foodie_connect_backend.Modules.RestaurantServices.Dtos;

public class RestaurantServiceResponseDto
{
    public Guid RestaurantId { get; init; }
    public string Name { get; init; } = null!;
}