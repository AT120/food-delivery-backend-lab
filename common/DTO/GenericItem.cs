using System.ComponentModel.DataAnnotations;

namespace ProjCommon.DTO;

public class GenericItem
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
}