using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PromptArchive.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Prompt> Prompts { get; set; }
    public DbSet<PromptVersion> PromptVersions { get; set; }
    public DbSet<PromptComment> PromptComments { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<PromptModel> PromptModels { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PromptTag> PromptTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PromptTag>()
            .HasKey(pt => new { pt.PromptId, pt.TagId });

        builder.Entity<PromptTag>()
            .HasOne(pt => pt.Prompt)
            .WithMany(p => p.PromptTags)
            .HasForeignKey(pt => pt.PromptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PromptTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptModel>()
            .HasKey(pt => new { pt.PromptId, pt.ModelId });
        
        builder.Entity<PromptModel>()
            .HasOne(pt => pt.Prompt)
            .WithMany(p => p.PromptModels)
            .HasForeignKey(pt => pt.PromptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptModel>()
            .HasOne(pt => pt.Model)
            .WithMany(t => t.PromptModels)
            .HasForeignKey(pt => pt.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Tag>()
            .HasIndex(t => t.NormalizedName)
            .IsUnique();

        builder.Entity<Model>()
            .HasIndex(m => m.NormalizedName)
            .IsUnique();

        builder.Entity<PromptVersion>()
            .HasOne(v => v.Prompt)
            .WithMany(p => p.PromptVersions)
            .HasForeignKey(v => v.PromptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Prompts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        builder.Entity<PromptComment>()
            .HasOne(c => c.Prompt)
            .WithMany(p => p.Comments)
            .HasForeignKey(p => p.PromptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromptComment>()
            .HasOne(c => c.User)
            .WithMany(u => u.PromptComments)
            .OnDelete(DeleteBehavior.Cascade);
    }
}