
namespace ProjCommon.Enums;

public enum OrderStatus
{
    Created             = 0b00000001,
    Kitchen             = 0b00000010,
    Packaging           = 0b00000100,
    AwaitsCourier       = 0b00001000,
    Delivery            = 0b00010000,
    Delivered           = 0b00100000,
    Canceled            = 0b01000000,

    Completed = Delivered + Canceled,   
}

public static class OrderStatusNames
{
    public static IReadOnlyDictionary<OrderStatus, string> Names 
        = new Dictionary<OrderStatus, string> {
            {OrderStatus.Created,       "Создан"},
            {OrderStatus.Kitchen,       "Готовится"},
            {OrderStatus.Packaging,     "Упаковывается"},
            {OrderStatus.AwaitsCourier, "Ожидает курьера"},
            {OrderStatus.Delivery,      "В доставке"},
            {OrderStatus.Delivered,     "Доставлен"},
            {OrderStatus.Canceled,      "Отменен"},
        };
}