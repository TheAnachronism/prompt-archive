import api from './api';

export interface Prompt {
    id: string;
    title: string;
    description: string;
    createdAt: string;
    updatedAt: string;
    userId: string;
    userName: string;
    tags: string[];
    models: string[];
    latestVersion?: PromptVersion;
    versionCount: number;
    commentCount: number;
}

export interface PromptVersion {
    id: string;
    promptId: string;
    promptContent: string;
    versionNumber: number;
    createdAt: string;
    userId: string;
    userName: string;
    images: PromptImage[];
}

export interface PromptImage {
    id: string;
    imageUrl: string;
    caption?: string;
    createdAt: string;
    originalFileName: string;
    fileSizeBytes: number;
}

export interface PromptComment {
    id: string;
    promptId: string;
    content: string;
    createdAt: string;
    updatedAt?: string;
    userId: string;
    userName: string;
}

export interface Tag {
    id: string;
    name: string;
    promptCount: number;
}

export interface Model {
    id: string;
    name: string;
    promptCount: number;
}

export interface PromptListResponse {
    prompts: Prompt[];
    totalCount: number;
    pageSize: number;
    currentPage: number;
}

export interface CreatePromptRequest {
    title: string;
    description: string;
    promptContent: string;
    tags: string[];
    models: string[];
    images?: File[];
    imageCaptions?: Record<string, string>
}

export interface UpdatePromptRequest {
    title: string;
    description: string;
    tags: string[];
    models: string[]
}

export interface CreateVersionRequest {
    promptContent: string;
    images?: File[];
    imageCaptions?: Record<string, string>
}

export interface CommentRequest {
    content: string;
}

export const promptService = {
    getPrompts: async (page = 1, pageSize = 10, searchTerm: string | undefined, userId: string | undefined, models: string[], tags: string[]): Promise<PromptListResponse> => {
        const params = new URLSearchParams();
        params.append('page', page.toString());
        params.append('pageSize', pageSize.toString());
        if (searchTerm) params.append('searchTerm', searchTerm);
        if (userId) params.append('userId', userId);
        models.forEach(m => params.append('models', m));
        tags.forEach(t => params.append('tags', t));

        const { data } = await api.get(`prompts?${params.toString()}`);
        return data;
    },
    getPromptById: async (id: string): Promise<Prompt> => {
        const { data } = await api.get(`prompts/${id}`);
        return data;
    },
    createPrompt: async (prompt: CreatePromptRequest): Promise<Prompt> => {
        const formData = new FormData();
        formData.append('title', prompt.title)
        formData.append('description', prompt.description);
        formData.append('promptContent', prompt.promptContent);

        prompt.tags.forEach(t => formData.append('tags', t));
        prompt.models.forEach(m => formData.append('models', m));

        if (prompt.images) {
            prompt.images.forEach(i => {
                formData.append('images', i);
            });

            if (prompt.imageCaptions && Object.keys(prompt.imageCaptions).length > 0) {
                formData.append('ImageCaptionsJson', JSON.stringify(prompt.imageCaptions));
            }
        }

        const { data } = await api.post('prompts', formData, {
            headers: {
                "Content-Type": 'multipart/form-data'
            }
        });
        return data;
    },
    updatePrompt: async (id: string, prompt: UpdatePromptRequest): Promise<Prompt> => {
        const { data } = await api.put(`prompts/${id}`, prompt);
        return data;
    },
    deletePrompt: async (id: string): Promise<void> => {
        await api.delete(`prompts/${id}`);
    },
    getPromptVersions: async (promptId: string): Promise<PromptVersion[]> => {
        const { data } = await api.get(`/prompts/${promptId}/versions`);
        return data;
    },

    getVersionById: async (versionId: string): Promise<PromptVersion> => {
        const { data } = await api.get(`/prompt-versions/${versionId}`);
        return data;
    },

    createVersion: async (promptId: string, version: CreateVersionRequest): Promise<PromptVersion> => {
        const formData = new FormData();
        formData.append('promptContent', version.promptContent);

        if (version.images) {
            version.images.forEach(image => {
                formData.append('images', image);
            });
            if (version.imageCaptions && Object.keys(version.imageCaptions).length > 0) {
                formData.append('ImageCaptionsJson', JSON.stringify(version.imageCaptions));
            }
        }
        const { data } = await api.post(`/prompts/${promptId}/versions`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        return data;
    },

    addImagesToVersion: async (versionId: string, images: File[], captions?: Record<string, string>): Promise<void> => {
        const formData = new FormData();

        images.forEach(image => {
            formData.append('images', image);
        });

        if (captions && Object.keys(captions).length > 0) {
            formData.append('ImageCaptionsJson', JSON.stringify(captions));
        }

        await api.post(`/prompts/versions/${versionId}/images`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    },

    deleteVersionImage: async (imageId: string): Promise<void> => {
        await api.delete(`/prompts/versions/images/${imageId}`);
    },

    getComments: async (promptId: string): Promise<PromptComment[]> => {
        const { data } = await api.get(`/prompts/${promptId}/comments`);
        return data;
    },

    addComment: async (promptId: string, comment: CommentRequest): Promise<PromptComment> => {
        const { data } = await api.post(`/prompts/${promptId}/comments`, comment);
        return data;
    },

    updateComment: async (commentId: string, comment: CommentRequest): Promise<PromptComment> => {
        const { data } = await api.put(`/comments/${commentId}`, comment);
        return data;
    },

    deleteComment: async (commentId: string): Promise<void> => {
        await api.delete(`/comments/${commentId}`);
    },

    getTags: async (): Promise<Tag[]> => {
        const { data } = await api.get('/tags');
        return data;
    },
    getModels: async (): Promise<Model[]> => {
        const { data } = await api.get('/models');
        return data;
    }
};