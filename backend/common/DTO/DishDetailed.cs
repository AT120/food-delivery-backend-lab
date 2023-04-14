using BackendCommon.Enums;

namespace BackendCommon.DTO;

public class DishDetailed
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public uint Price { get; set; }
    public string Description { get; set; }
    public bool IsVegetarian { get; set; }
    public float Rating { get; set; }
    public DishCategory Category { get; set; }
    public string PhotoUrl { get; set; }
    public bool CanBeRated { get; set; }

}
