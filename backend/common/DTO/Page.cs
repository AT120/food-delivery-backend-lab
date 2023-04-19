namespace BackendCommon.DTO;

public class Page<T>
{
    public PageInfo PageInfo { get; set; }
    public ICollection<T> Items { get; set; }
}