namespace BackendDAL.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Menu> Menus { get; set; }
    public ICollection<Guid> Cooks { get; set; }
    public ICollection<Guid> Managers { get; set; }


}