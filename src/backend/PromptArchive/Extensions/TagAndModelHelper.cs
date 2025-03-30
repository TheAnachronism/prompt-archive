using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PromptArchive.Database;

namespace PromptArchive.Extensions;

public static class TagAndModelHelper
{
    public static  async IAsyncEnumerable<Tag> GetAndEnsureTags(ApplicationDbContext dbContext, IEnumerable<string> tags,
        [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var tagName in tags)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                continue;

            var normalizedName = tagName.ToLower();
            var tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, ct);

            if (tag is null)
            {
                tag = new Tag
                {
                    Name = tagName,
                    NormalizedName = normalizedName
                };

                dbContext.Tags.Add(tag);
            }

            yield return tag;
        }
    }

    public static async IAsyncEnumerable<Model> GetAndEnsureModels(ApplicationDbContext dbContext, IEnumerable<string> models,
        [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var modelName in models)
        {
            if (string.IsNullOrWhiteSpace(modelName))
                continue;

            var normalizedName = modelName.ToLower();
            var model = await dbContext.Models.FirstOrDefaultAsync(m => m.NormalizedName == normalizedName, ct);

            if (model is null)
            {
                model = new Model
                {
                    NormalizedName = normalizedName,
                    Name = modelName
                };

                dbContext.Models.Add(model);
            }

            yield return model;
        }
    }
}