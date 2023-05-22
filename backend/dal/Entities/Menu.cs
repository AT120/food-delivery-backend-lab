namespace BackendDAL.Entities;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Dish> Dishes { get; set; }

    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }

    public bool Archivied { get; set; } = false;
}