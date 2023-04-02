using System.Diagnostics;

namespace BackendApi.Models;

public enum OrderFilterStatus
{
    InProgress = 1 << 0,
    Delivered = 1 << 1,
    Canceled = 1 << 2,
    Uncanceled = InProgress + Delivered,
    Undelivered = InProgress + Canceled,
    Completed = Delivered + Canceled,
    All = InProgress + Delivered + Canceled,
}