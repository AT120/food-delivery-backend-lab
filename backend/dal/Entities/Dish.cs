using System.ComponentModel.DataAnnotations;
using BackendCommon.Enums;

namespace BackendDAL.Entities;

public class Dish
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    
    [Range(0, int.MaxValue)]
    public int Price { get; set; }
    public string Description { get; set; }  // changable
    public bool IsVegetarian { get; set; }

    [Url]
    public string PhotoURL { get; set; }  // changable
    public DishCategory Category { get; set; }  // changable

    public Guid? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public ICollection<Menu> Menus { get; set; }
    
    [Range(0, 10)]
    public double Rating { get; set; }
    public int PeopleRated { get; set; }

    public bool Archived { get; set; }


}