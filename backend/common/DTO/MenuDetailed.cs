namespace BackendCommon.DTO;

public class MenuDetailed
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<DishShort> Dishes { get; set; }
}