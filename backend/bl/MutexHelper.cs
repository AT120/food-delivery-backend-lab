namespace BackendBl;

public static class MutexHelper
{
    public static Mutex OrderMutex(int orderId) => new Mutex(false, $"order-{orderId}");
}