namespace BackendCommon.Enums;

public enum OrderStatus
{
    Created             = 0b00000001,
    Kitchen             = 0b00000010,
    Packaging           = 0b00000100,
    AwaitsForCourier    = 0b00001000,
    Delivery            = 0b00010000,
    Delivered           = 0b00100000,
    Canceled            = 0b01000000,

    Completed = Delivered + Canceled,
    
}