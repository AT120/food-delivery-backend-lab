using System.ComponentModel.DataAnnotations;

namespace BackendDAL.Entities;

public class RatedDish
{
    public Guid CustomerId { get; set; }
    public Guid DishId { get; set; }
    public Dish Dish { get; set; }
    
    [Range(0, 10)]
    public int? Rating { get; set; }
}