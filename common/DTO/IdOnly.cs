namespace ProjCommon.DTO;

public class IdOnly
{
    public Guid Id { get; set; }

    public IdOnly() { }

    public IdOnly(Guid id)
    {
        Id = id;
    }
}
