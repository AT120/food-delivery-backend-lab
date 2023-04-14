namespace BackendCommon.DTO;

public class CartDTO
{
    public ICollection<DishInCartDTO> Dishes { get; set; }
    public int FinalPrice { get; set; }
}
