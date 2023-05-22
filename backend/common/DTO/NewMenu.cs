namespace BackendCommon.DTO;

public class NewMenu
{
    public required string Name { get; set; }
    public IEnumerable<Guid>? Dishes { get; set; }
}