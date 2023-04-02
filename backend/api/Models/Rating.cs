using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Rating
{   
    [Range(0, 10)]
    public int value { get; set; }
}