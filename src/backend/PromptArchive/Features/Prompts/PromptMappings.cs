using PromptArchive.Database;
using PromptArchive.Features.Prompts.Get;
using PromptArchive.Services;

namespace PromptArchive.Features.Prompts;

public static class PromptMappings
{
    public static PromptResponse ToResponse(this Prompt prompt, IStorageService storageService)
    {
        return new PromptResponse(prompt.Id, prompt.Title, prompt.Description, prompt.CreatedAt, prompt.UpdatedAt,
            prompt.UserId, prompt.User.UserName!, prompt.Tags.Select(t => t.Name),
            prompt.Versions.Select(v => v.ToResponse(storageService)), prompt.Versions.Count, prompt.Comments.Count);
    }

    public static PromptVersionResponse ToResponse(this PromptVersion version, IStorageService storageService)
    {
        return new PromptVersionResponse(version.Id, version.PromptId, version.Content, version.VersionHash,
            version.CreatedAt, version.UserId, version.User.UserName!,
            version.Images.Select(i => i.ToResponse(storageService)));
    }

    public static PromptImageResponse ToResponse(this PromptImage image, IStorageService storageService)
    {
        return new PromptImageResponse(image.Id, storageService.GetImageUrl(image.ImagePath), image.FileName,
            image.Caption, image.DisplayOrder);
    }
}