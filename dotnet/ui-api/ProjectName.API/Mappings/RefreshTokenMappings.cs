namespace ProjectName.API.Mappings;
public static class RefreshTokenMappings
{
    public static RefreshTokenDto ToDTO(this RefreshToken model)
    {
        return new() {
			Id = model.Id,
			UserId = model.UserId,
			Token = model.Token,
			ExpiresOn = model.ExpiresOn,
            CreatedBy = model.CreatedBy,
            CreatedOn = model.CreatedOn,
            ModifiedBy = model.ModifiedBy,
            ModifiedOn = model.ModifiedOn,
            IsDeleted = model.IsDeleted,
        };
    }

    public static RefreshToken ToModel(this RefreshTokenDto dto, Guid? id = null)
    {
        return new(
            id ?? new(),
			dto.UserId,
			dto.Token,
			dto.ExpiresOn
        );
    }
}
