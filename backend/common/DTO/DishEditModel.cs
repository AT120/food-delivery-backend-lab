using System.ComponentModel.DataAnnotations;
using BackendCommon.Enums;
using EasyNetQ.Events;
using EasyNetQ.Producer;

namespace BackendCommon.DTO;

public class DishEdit
{
    [Range(0, int.MaxValue)]
    public int? Price { get; set; }
    public string? Description { get; set; }
    public bool? IsVegetarian { get; set; }

    [Url]
    public string? PhotoURL { get; set; }
    public DishCategory? Category { get; set; }
    public bool? Archived { get; set; }

}