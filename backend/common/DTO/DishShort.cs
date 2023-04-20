namespace BackendCommon.DTO;

public class DishShort
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public bool IsVegetarian { get; set; }
    public string PhotoUrl { get; set; }
}
