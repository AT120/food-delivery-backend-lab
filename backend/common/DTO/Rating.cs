using System.ComponentModel.DataAnnotations;

namespace BackendCommon.DTO;

public class Rating
{   
    [Range(0, 10)]
    public int value { get; set; }
}
