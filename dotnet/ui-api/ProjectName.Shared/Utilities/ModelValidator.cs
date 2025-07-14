namespace ProjectName.Shared.Utilities;

public abstract class ModelValidator
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }

    public abstract Dictionary<string, string>? Errors();
    public bool IsValid => Errors() is null || Errors()?.Count == 0;
}