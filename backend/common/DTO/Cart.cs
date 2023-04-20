namespace BackendCommon.DTO;

public class CartDTO
{
    public ICollection<DishInCart> Dishes { get; set; }
    public int FinalPrice { get; set; }
}
