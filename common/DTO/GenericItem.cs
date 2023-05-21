using System.ComponentModel.DataAnnotations;

namespace ProjCommon.DTO;

public class GenericItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}