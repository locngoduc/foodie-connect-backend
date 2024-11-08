using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Shared.Enums;

namespace foodie_connect_backend.Data;

public class DishReview
{
    public Guid Id { get; set; }
    public Guid DishId { get; set; }
}