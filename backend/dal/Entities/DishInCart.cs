using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace BackendDAL.Entities;

public class DishInCart
{
    public Guid CustomerId { get; set; }

    public Guid DishId { get; set; }
    public Dish Dish { get; set; }


    public int Count { get; set; }
    
}