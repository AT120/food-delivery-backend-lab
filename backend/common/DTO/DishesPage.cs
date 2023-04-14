namespace BackendCommon.DTO;

public class DishesPage
{
    public PageInfo Page { get; set; }
    public ICollection<DishShort> Dishes { get; set; }
}