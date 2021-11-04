using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NpgsqlArrayMappingIssue;

public class TestEntity
{
    public TestEntity(Guid id, IntWrapper[] values)
    {
        Id = id;
        Values = values;
    }

    public Guid Id { get; }
    public IntWrapper[] Values { get; }
}

public class IntWrapper
{
    public int Value { get; }

    public IntWrapper(int value)
    {
        Value = value;
    }
}

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; } = default!;
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<TestEntity>()
            .Property(x => x.Values)
            .HasPostgresArrayConversion(f => f.Value, w => new IntWrapper(w));
    }
}

public class TestDbContextContextDesignFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        optionsBuilder.UseNpgsql("default");
        return new TestDbContext(optionsBuilder.Options);
    }
}
