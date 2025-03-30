import { defineStore } from 'pinia';
import { ref, computed, onErrorCaptured } from 'vue';
import {
    promptService,
    type Prompt,
    type PromptVersion,
    type PromptComment,
    type Tag,
    type Model,
    type CreatePromptRequest,
    type UpdatePromptRequest,
    type CreateVersionRequest,
    type CommentRequest
} from '@/utils/promptService';

export const usePromptStore = defineStore('prompt', () => {
    const prompts = ref<Prompt[]>([]);
    const currentPrompt = ref<Prompt | null>(null);
    const versions = ref<PromptVersion[]>([]);
    const comments = ref<PromptComment[]>([]);
    const tags = ref<Tag[]>([]);
    const models = ref<Model[]>([]);
    const totalCount = ref(0);
    const currentPage = ref(1);
    const pageSize = ref(10);
    const isLoading = ref(false);
    const error = ref<string | null>(null);

    const hasNextPage = computed(() => {
        return currentPage.value * pageSize.value < totalCount.value;
    });

    const hasPreviousPage = computed(() => {
        return currentPage.value > 1;
    });

    async function fetchPrompts(page = 1, size = 10, searchTerm: string | undefined, models: string[], tags: string[], userId: string | undefined) {
        isLoading.value = true;
        error.value = null;

        try {
            const response = await promptService.getPrompts(page, size, searchTerm, userId, models, tags);
            prompts.value = response.prompts;
            totalCount.value = response.totalCount;
            currentPage.value = response.currentPage;
            pageSize.value = response.pageSize;
        } catch (err) {
            error.value = 'Failed to load prompts';
            console.error('Error fetching prompts:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchPromptById(id: string) {
        isLoading.value = true;
        error.value = null;

        try {
            currentPrompt.value = await promptService.getPromptById(id);
        } catch (err) {
            error.value = 'Failed to load prompt';
            console.error('Error fetching prompt:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function createPrompt(prompt: CreatePromptRequest) {
        isLoading.value = true;
        error.value = null;

        try {
            const newPrompt = await promptService.createPrompt(prompt);
            return newPrompt;
        } catch (err) {
            error.value = 'Failed to create prompt';
            console.error('Error creating prompt:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function updatePrompt(id: string, prompt: UpdatePromptRequest) {
        isLoading.value = true;
        error.value = null;

        try {
            const updatedPrompt = await promptService.updatePrompt(id, prompt);
            if (currentPrompt.value?.id === id) {
                currentPrompt.value = updatedPrompt;
            }
            return updatedPrompt;
        } catch (err) {
            error.value = 'Failed to update prompt';
            console.error('Error updating prompt:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function deletePrompt(id: string) {
        isLoading.value = true;
        error.value = null;

        try {
            await promptService.deletePrompt(id);
            if (currentPrompt.value?.id === id) {
                currentPrompt.value = null;
            }
            prompts.value = prompts.value.filter(p => p.id !== id);
        } catch (err) {
            error.value = 'Failed to delete prompt';
            console.error('Error deleting prompt:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchVersions(promptId: string) {
        isLoading.value = true;
        error.value = null;

        try {
            versions.value = await promptService.getPromptVersions(promptId);
        } catch (err) {
            error.value = 'Failed to load versions';
            console.error('Error fetching versions:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function createVersion(promptId: string, version: CreateVersionRequest) {
        isLoading.value = true;
        error.value = null;

        try {
            const newVersion = await promptService.createVersion(promptId, version);
            versions.value = [newVersion, ...versions.value];

            // Update current prompt if it's the one we're viewing
            if (currentPrompt.value?.id === promptId) {
                await fetchPromptById(promptId);
            }

            return newVersion;
        } catch (err) {
            error.value = 'Failed to create version';
            console.error('Error creating version:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function addImagesToVersion(versionId: string, images: File[], captions?: Record<string, string>) {
        isLoading.value = true;
        error.value = null;

        try {
            await promptService.addImagesToVersion(versionId, images, captions);

            // Refresh versions if we're viewing the prompt
            if (currentPrompt.value) {
                await fetchVersions(currentPrompt.value.id);
                await fetchPromptById(currentPrompt.value.id);
            }
        } catch (err) {
            error.value = 'Failed to add images to version';
            console.error('Error adding images to version:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function deletePromptVersion(versionId: string, promptId: string) {
        isLoading.value = true;
        error.value = null;

        try {
            await promptService.deletePromptVersion(versionId);

            // Refresh versions if we're viewing the prompt
            if (currentPrompt.value?.id === promptId) {
                await fetchVersions(promptId);
                await fetchPromptById(promptId);
            }
        } catch (err) {
            error.value = 'Failed to delete version';
            console.error('Error deleting version:', err);
            throw err;
        } finally {
            isLoading.value = true;
        }
    }

    async function deleteVersionImage(imageId: string, promptId: string) {
        isLoading.value = true;
        error.value = null;

        try {
            await promptService.deleteVersionImage(imageId);

            // Refresh versions if we're viewing the prompt
            if (currentPrompt.value?.id === promptId) {
                await fetchVersions(promptId);
                await fetchPromptById(promptId);
            }
        } catch (err) {
            error.value = 'Failed to delete image';
            console.error('Error deleting image:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchComments(promptId: string) {
        isLoading.value = true;
        error.value = null;

        try {
            comments.value = await promptService.getComments(promptId);
        } catch (err) {
            error.value = 'Failed to load comments';
            console.error('Error fetching comments:', err);
        } finally {
            isLoading.value = false;
        }
    }

    async function addComment(promptId: string, comment: CommentRequest) {
        isLoading.value = true;
        error.value = null;

        try {
            const newComment = await promptService.addComment(promptId, comment);
            comments.value = [newComment, ...comments.value];

            // Update comment count in current prompt
            if (currentPrompt.value?.id === promptId) {
                currentPrompt.value.commentCount += 1;
            }

            return newComment;
        } catch (err) {
            error.value = 'Failed to add comment';
            console.error('Error adding comment:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function updateComment(commentId: string, comment: CommentRequest) {
        isLoading.value = true;
        error.value = null;

        try {
            const updatedComment = await promptService.updateComment(commentId, comment);
            const index = comments.value.findIndex(c => c.id === commentId);
            if (index !== -1) {
                comments.value[index] = updatedComment;
            }
            return updatedComment;
        } catch (err) {
            error.value = 'Failed to update comment';
            console.error('Error updating comment:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function deleteComment(commentId: string, promptId: string) {
        isLoading.value = true;
        error.value = null;

        try {
            await promptService.deleteComment(commentId);
            comments.value = comments.value.filter(c => c.id !== commentId);

            // Update comment count in current prompt
            if (currentPrompt.value?.id === promptId) {
                currentPrompt.value.commentCount -= 1;
            }
        } catch (err) {
            error.value = 'Failed to delete comment';
            console.error('Error deleting comment:', err);
            throw err;
        } finally {
            isLoading.value = false;
        }
    }

    async function fetchTags(): Promise<Tag[]> {
        isLoading.value = true;
        error.value = null;

        try {
            tags.value = await promptService.getTags();
        } catch (err) {
            error.value = 'Failed to load tags';
            console.error('Error fetching tags:', err);
        } finally {
            isLoading.value = false;
            return tags.value;
        }
    }

    async function fetchModels(): Promise<Model[]> {
        isLoading.value = true;
        error.value = null;

        try {
            models.value = await promptService.getModels();
        } catch (err) {
            error.value = 'Failed to load tags';
            console.error('Error fetching tags:', err);
        } finally {
            isLoading.value = false;
            return models.value;
        }
    }

    return {
        prompts,
        currentPrompt,
        versions,
        comments,
        tags,
        models,
        totalCount,
        currentPage,
        pageSize,
        isLoading,
        error,
        hasNextPage,
        hasPreviousPage,
        fetchPrompts,
        fetchPromptById,
        createPrompt,
        updatePrompt,
        deletePrompt,
        fetchVersions,
        createVersion,
        deletePromptVersion,
        addImagesToVersion,
        deleteVersionImage,
        fetchComments,
        addComment,
        updateComment,
        deleteComment,
        fetchTags,
        fetchModels
    };
});
