namespace BackendCommon.DTO;

public class Cart
{
    public ICollection<DishInCart> Dishes { get; set; }
    public uint FinalPrice { get; set; }
}
