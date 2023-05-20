using ProjCommon.Enums;

namespace ProjCommon.DTO;

public class StatusChangedNotification
{
    public Guid UserId { get; set; }
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
}