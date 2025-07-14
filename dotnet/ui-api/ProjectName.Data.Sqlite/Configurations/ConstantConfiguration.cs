namespace ProjectName.Data.Sqlite.Configurations;

public class ConstantConfiguration : IEntityTypeConfiguration<Constant>
{
    void IEntityTypeConfiguration<Constant>.Configure(EntityTypeBuilder<Constant> builder)
    {
        builder.HasKey(p => p.Id);
    }
}

