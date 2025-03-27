import api from './api';

export interface UserProfile {
    id: string;
    userName: string;
    email: string;
    emailConfirmed: boolean;
    createdAt: string;
    lastLoginAt: string | null;
}

export interface UpdateProfileRequest {
    userName: string;
    email: string;
}

export interface ChangePasswordRequest {
    password: string;
    newPassword: string;
    newPasswordConfirm: string;
}

export const accountService = {
    getProfile: async (): Promise<UserProfile> => {
        const { data } = await api.get("/account/profile");
        return data;
    },
    updateProfile: async (profileData: UpdateProfileRequest): Promise<UserProfile> => {
        const { data } = await api.put("/account/profile", profileData);
        return data;
    },
    changePassword: async (request: ChangePasswordRequest): Promise<void> => {
        await api.post("/account/change-password", request);
    }
}