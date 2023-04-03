namespace BackendDAL.Entities;

public class Menu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Dish> Dishes { get; set; }
}