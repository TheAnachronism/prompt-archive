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
    url: string;
    fileName: string;
    caption?: string;
    displayOrder: number;
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
}

export interface UpdatePromptRequest {
    title: string;
    description: string;
    tags: string[];
    models: string[]
}

export interface CreateVersionRequest {
    content: string;
}

export interface CommentRequest {
    content: string;
}

export const promptService = {
    getPrompts: async (page = 1, pageSize = 10, searchTerm?: string, userId?: string): Promise<PromptListResponse> => {
        const params = new URLSearchParams();
        params.append('page', page.toString());
        params.append('pageSize', pageSize.toString());
        if (searchTerm) params.append('searchTerm', searchTerm);
        if (userId) params.append('userId', userId);

        const { data } = await api.get(`prompts?${params.toString()}`);
        return data;
    },
    getPromptById: async (id: string): Promise<Prompt> => {
        const { data } = await api.get(`prompts/${id}`);
        return data;
    },
    createPrompt: async (prompt: CreatePromptRequest): Promise<Prompt> => {
        const { data } = await api.post('prompts', prompt);
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
        const { data } = await api.post(`/prompts/${promptId}/versions`, version);
        return data;
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