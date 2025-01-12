namespace CoreNotify.API.Data.Entities.Conventions;

public abstract class BaseEntity
{
	public int Id { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
