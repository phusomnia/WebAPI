namespace WebAPI.core.entities;

public abstract class BaseEnity
{
    private DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    
    protected BaseEnity(){}

    protected BaseEnity(DateTime createdAt, DateTime updatedAt)
    {
        this.createdAt = createdAt;
        this.updatedAt = updatedAt;
    }
}