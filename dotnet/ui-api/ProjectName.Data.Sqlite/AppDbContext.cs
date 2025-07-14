namespace ProjectName.Data.Sqlite;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Add DbSet    
	public DbSet<Constant> Constants { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add Configuration
		modelBuilder.ApplyConfiguration(new ConstantConfiguration());
		modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}