namespace ProjectName.API.Mappings;
public static class ConstantMappings
{
    public static ConstantDto ToDTO(this Constant model)
    {
        return new() {
			Id = model.Id,
			Order = model.Order,
			Group = model.Group,
			Key = model.Key,
			Value = model.Value,
            CreatedBy = model.CreatedBy,
            CreatedOn = model.CreatedOn,
            ModifiedBy = model.ModifiedBy,
            ModifiedOn = model.ModifiedOn,
            IsDeleted = model.IsDeleted,
        };
    }

    public static Constant ToModel(this ConstantDto dto, Guid? id = null)
    {
        return new(
            id ?? new(),
			dto.Order,
			dto.Group,
			dto.Key,
			dto.Value
        );
    }
}
