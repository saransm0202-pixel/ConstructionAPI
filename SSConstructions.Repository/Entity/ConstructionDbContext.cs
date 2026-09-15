using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity.EntityClass;

namespace SSConstructions.Repository.Entity;

public partial class ConstructionDbContext : DbContext
{
    public ConstructionDbContext()
    {
    }

    public ConstructionDbContext(DbContextOptions<ConstructionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<MasterRole> MasterRoles { get; set; }
    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<AppConfig> AppConfigs { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA5A6FA138693");
        });
        modelBuilder.Entity<AppConfig>(entity =>
        {
            entity.HasKey(e => e.AppConfigId).HasName("PK__AppConfi__3BA19DFAE295FDF8");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Account).WithMany(p => p.AppConfigs).HasConstraintName("FK_AppConfig_Accounts");
        });

        modelBuilder.Entity<MasterRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Master.R__8AFACE1AC6BE0767");
        });
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0D2924BDA");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Account).WithMany(p => p.Projects).HasConstraintName("FK_Projects_Accounts");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Account).WithMany(p => p.Users).HasConstraintName("FK_Users_Accounts");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK_Users_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
