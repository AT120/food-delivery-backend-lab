using System.ComponentModel.DataAnnotations;

namespace BackendCommon.DTO;

public class Rating
{   
    [Range(1, 10)]
    public int Value { get; set; }
}
