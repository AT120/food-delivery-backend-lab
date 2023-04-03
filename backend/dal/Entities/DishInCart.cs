namespace BackendDAL.Entities;

public class DishInCart
{
    public Dish Dish { get; set; }
    public Customer Customer { get; set; }
    public int Count { get; set; }
    
}