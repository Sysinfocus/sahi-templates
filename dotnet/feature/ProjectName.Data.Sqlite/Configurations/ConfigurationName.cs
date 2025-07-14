namespace ProjectName.Data.Sqlite.Configurations;

public class {{DTO}}Configuration : IEntityTypeConfiguration<{{DTO}}>
{
    void IEntityTypeConfiguration<{{DTO}}>.Configure(EntityTypeBuilder<{{DTO}}> builder)
    {
        builder.HasKey(p => p.Id);
    }
}

