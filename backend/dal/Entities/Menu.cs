namespace BackendDAL.Entities;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();

    public Guid? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public bool Archived { get; set; } = false;
}