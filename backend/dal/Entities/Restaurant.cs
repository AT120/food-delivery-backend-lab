namespace BackendDAL.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Dish> Dishes { get; set; }
    public ICollection<Menu> Menus { get; set; }
    public ICollection<Cook> Cooks { get; set; }
    public ICollection<Manager> Managers { get; set; }


}