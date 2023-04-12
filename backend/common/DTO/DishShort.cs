namespace BackendCommon.DTO;

public class DishShort //TODO: rename
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public uint Price { get; set; }
    public bool IsVegetarian { get; set; }
    public string PhotoUrl { get; set; }
}
