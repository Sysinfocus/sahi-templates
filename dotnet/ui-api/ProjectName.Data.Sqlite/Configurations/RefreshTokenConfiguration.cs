namespace ProjectName.Data.Sqlite.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    void IEntityTypeConfiguration<RefreshToken>.Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(p => p.Id);
    }
}

