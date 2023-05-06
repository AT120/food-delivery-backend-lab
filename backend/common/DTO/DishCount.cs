using System.ComponentModel.DataAnnotations;

namespace BackendCommon.DTO;

public class DishCount
{
    public Guid Id { get; set; }
    [Range(0, int.MaxValue)]
    public int Count { get; set; }
}
