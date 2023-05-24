using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BackendCommon.Enums;

namespace BackendCommon.DTO;

public class DishCreate
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    [Range(0, int.MaxValue)]
    public int Price { get; set; }
    public bool IsVegetarian { get; set; }
    [Url]
    public required string PhotoURL { get; set; }
    public DishCategory Category { get; set; }
    public IEnumerable<int> MenuIds { get; set; } = new List<int>();
}