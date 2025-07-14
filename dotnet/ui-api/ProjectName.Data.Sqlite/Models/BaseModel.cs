namespace ProjectName.Data.Sqlite.Models;
public record BaseModel
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
}
