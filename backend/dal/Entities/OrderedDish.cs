namespace BackendDAL.Entities;

public class OrderedDish
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public Guid DishId { get; set; }
    public Dish Dish { get; set; }
    
    public int Count { get; set; }
}