using System.ComponentModel.DataAnnotations;
using BackendCommon.Enums;

namespace BackendDAL.Entities;

public class Dish
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string Description { get; set; }
    public bool IsVegetarian { get; set; }

    [Url]
    public string PhotoURL { get; set; }
    public DishCategory Category { get; set; }

    [Range(0, 10)]
    public double Rating { get; set; }
    public int PeopleRated { get; set; }

}