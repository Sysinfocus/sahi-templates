namespace ProjectName.API.Mappings;
public static class UserMappings
{
    public static UserDto ToDTO(this User model)
    {
        return new() {
			Id = model.Id,
			Username = model.Username,
			Password = model.Password,
			Fullname = model.Fullname,
			Email = model.Email,
			Roles = model.Roles,
            IsLocked = model.IsLocked,
            CreatedBy = model.CreatedBy,
            CreatedOn = model.CreatedOn,
            ModifiedBy = model.ModifiedBy,
            ModifiedOn = model.ModifiedOn,
            IsDeleted = model.IsDeleted,
        };
    }

    public static User ToModel(this UserDto dto, Guid? id = null)
    {
        return new(
            id ?? new(),
			dto.Username,
			dto.Password,
			dto.Fullname,
			dto.Email,
			dto.Roles,
            dto.IsLocked
        );
    }

    public static User ToModel(this UpdateUserDto dto, Guid? id = null)
    {
        return new(
            id ?? new(),
            dto.Username,
            string.Empty,
            dto.Fullname,
            dto.Email,
            dto.Roles,
            dto.IsLocked
        );
    }
}
