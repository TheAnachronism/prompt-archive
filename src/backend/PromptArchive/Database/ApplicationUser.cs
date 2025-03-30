using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace PromptArchive.Database;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<Prompt> Prompts { get; set; } = new List<Prompt>();
    public ICollection<PromptComment> PromptComments { get; set; } = new List<PromptComment>();
}