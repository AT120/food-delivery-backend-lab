namespace AdminCommon.DTO;

public class UserId
{
    public Guid Id { get; set; }

    public UserId() { }

    public UserId(Guid Id)
    {
        this.Id = Id;
    }
}