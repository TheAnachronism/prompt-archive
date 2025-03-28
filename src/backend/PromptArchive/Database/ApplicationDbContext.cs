using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PromptArchive.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Prompt> Prompts { get; set; } = null!;
    public DbSet<PromptVersion> PromptVersions { get; set; } = null!;
    public DbSet<PromptImage> PromptImages { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<PromptComment> PromptComments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tag>()
            .HasMany(x => x.Prompts)
            .WithMany(x => x.Tags);

        builder.Entity<Tag>()
            .HasIndex(x => x.NormalizedName)
            .IsUnique();

        builder.Entity<PromptVersion>()
            .HasOne(x => x.Prompt)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.PromptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptImage>()
            .HasOne(x => x.PromptVersion)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PromptVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptComment>()
            .HasOne(x => x.Prompt)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.PromptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}