import api from './api';

export interface User {
    id: string;
    userName: string;
    email: string;
    roles: string[];
    emailConfirmed: boolean;
    createdAt: string;
    lastLoginAt: string | null;
}

export interface UserListResponse {
    users: User[];
    totalCount: number;
    pageSize: number;
    currentPage: number;
}

export interface CreateUserRequest {
    userName: string;
    email: string;
    password: string;
    roles: string[];
}

export interface UpdateUserRequest {
    userName: string;
    email: string;
    roles: string[];
    emailConfirmed: boolean;
}

export interface ChangePasswordRequest {
    userId: string;
    newPassword: string;
}

export const userService = {
    getUsers: async (page = 1, pageSize = 10, searchTerm?: string): Promise<UserListResponse> => {
        const params = new URLSearchParams();
        params.append('page', page.toString());
        params.append('pageSize', pageSize.toString());
        if (searchTerm) {
            params.append('searchTerm', searchTerm);
        }
        const { data } = await api.get(`/manage/users?${params.toString()}`);
        return data;
    },

    getUser: async (id: string): Promise<User> => {
        const { data } = await api.get(`/manage/users/${id}`);
        return data;
    },

    createUser: async (user: CreateUserRequest): Promise<User> => {
        const { data } = await api.post('/manage/users', user);
        return data;
    },

    updateUser: async (id: string, user: UpdateUserRequest): Promise<User> => {
        const { data } = await api.put(`/manage/users/${id}`, user);
        return data;
    },

    deleteUser: async (id: string): Promise<void> => {
        await api.delete(`/manage/users/${id}`);
    },

    changePassword: async (request: ChangePasswordRequest): Promise<void> => {
        await api.post('/manage/users/change-password', request);
    },

    getRoles: async (): Promise<string[]> => {
        const { data } = await api.get('/manage/roles');
        return data;
    }
};