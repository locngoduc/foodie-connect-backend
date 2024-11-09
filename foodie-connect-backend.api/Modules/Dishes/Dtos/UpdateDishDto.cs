namespace foodie_connect_backend.Modules.Dishes.Dtos;

public class UpdateDishDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; } = 0;
    public string[] Categories { get; set; } = [];
}