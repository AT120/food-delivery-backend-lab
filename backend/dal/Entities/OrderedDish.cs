using System.ComponentModel.DataAnnotations;

namespace BackendDAL.Entities;

public class OrderedDish
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public Guid DishId { get; set; }
    public Dish Dish { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Count { get; set; }

    [Range(0, int.MaxValue)]
    public int DishPrice { get; set; }
}