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
    public virtual DbSet<Package> Packages { get; set; }
    public virtual DbSet<PackageFeature> PackageFeatures { get; set; }
    public virtual DbSet<PackageSpec> PackageSpecs { get; set; }
    public virtual DbSet<MasterConstructionType> MasterConstructionTypes { get; set; }
    public virtual DbSet<MasterPackageSpecType> MasterPackageSpecTypes { get; set; }

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
        modelBuilder.Entity<MasterConstructionType>(entity =>
        {
            entity.HasKey(e => e.ConstructionTypeId).HasName("PK__Master.C__5793AC770091551F");
        });
        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Packages__322035CCD4BADF04");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Account).WithMany(p => p.Packages).HasConstraintName("FK_Package_Accounts");

            entity.HasOne(d => d.ConstructionType).WithMany(p => p.Packages).HasConstraintName("FK_Package_ConstructionType");
        });
        modelBuilder.Entity<PackageFeature>(entity =>
        {
            entity.HasKey(e => e.PackageFeatureId).HasName("PK__PackageF__72F359A6AF89F5EB");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Package).WithMany(p => p.PackageFeatures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feature_PackageId");
        });
        modelBuilder.Entity<PackageSpec>(entity =>
        {
            entity.HasKey(e => e.PackageSpecId).HasName("PK__PackageS__5DE04D6473D2E9C8");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Package).WithMany(p => p.PackageSpecs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Spec_PackageId");

            entity.HasOne(d => d.SpecType).WithMany(p => p.PackageSpecs)
                .HasForeignKey(d => d.SpecTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Package_SpecTypeId");
        });
        modelBuilder.Entity<MasterPackageSpecType>(entity =>
        {
            entity.HasKey(e => e.PackageSpecTypeId).HasName("PK__Master.P__D9D8702F5BA6A337");
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
