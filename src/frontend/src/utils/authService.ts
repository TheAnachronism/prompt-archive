import api from './api'
import { ref } from 'vue'

export interface User {
    id: string;
    email: string;
    userName: string;
}

export interface LoginCredentials {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface RegisterCredentials {
    email: string;
    userName: string;
    password: string;
    passwordConfirm: string;
}

const currentUser = ref<User | null>(null);
const isAuthenticated = ref<boolean>(false);
const isLoading = ref<boolean>(false);

export const useAuth = () => {
    // Check if user is logged in
    const checkAuthStatus = async (): Promise<void> => {
        isLoading.value = true;
        try {
            const { data } = await api.get('/auth/me');
            currentUser.value = data;
            isAuthenticated.value = true;
        } catch (error) {
            currentUser.value = null;
            isAuthenticated.value = false;
        } finally {
            isLoading.value = false;
        }
    };

    // Login user
    const login = async (credentials: LoginCredentials): Promise<User> => {
        const { data } = await api.post('/auth/login', credentials);
        currentUser.value = data;
        isAuthenticated.value = true;
        return data;
    };

    // Register user
    const register = async (credentials: RegisterCredentials): Promise<User> => {
        const { data } = await api.post('/auth/register', credentials);
        currentUser.value = data;
        isAuthenticated.value = true;
        return data;
    };

    // Logout user
    const logout = async (): Promise<void> => {
        await api.post('/auth/logout');
        currentUser.value = null;
        isAuthenticated.value = false;
    };

    // Reset password
    const forgotPassword = async (email: string): Promise<void> => {
        await api.post('/auth/forgot-password', { email });
    };

    return {
        currentUser,
        isAuthenticated,
        isLoading,
        checkAuthStatus,
        login,
        register,
        logout,
        forgotPassword
    };
};