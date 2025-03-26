import { defineStore } from 'pinia'
import { useAuth, type LoginCredentials, type RegisterCredentials } from '@/utils/authService'

export const useAuthStore = defineStore('auth', () => {
    const {
        currentUser,
        isAuthenticated,
        isLoading,
        checkAuthStatus,
        login: apiLogin,
        register: apiRegister,
        logout: apiLogout,
        forgotPassword: apiForgotPassword
    } = useAuth()

    async function initialize() {
        await checkAuthStatus()
    }

    async function login(credentials: LoginCredentials) {
        try {
            await apiLogin(credentials)
            return true
        } catch (error) {
            console.error('Login failed:', error)
            return false
        }
    }

    async function register(credentials: RegisterCredentials) {
        try {
            await apiRegister(credentials)
            return true
        } catch (error) {
            console.error('Registration failed:', error)
            return false
        }
    }

    async function logout() {
        try {
            await apiLogout()
            return true
        } catch (error) {
            console.error('Logout failed:', error)
            return false
        }
    }

    async function forgotPassword(email: string) {
        try {
            await apiForgotPassword(email)
            return true
        } catch (error) {
            console.error('Password reset request failed:', error)
            return false
        }
    }

    return {
        user: currentUser,
        isAuthenticated,
        isLoading,
        initialize,
        login,
        register,
        logout,
        forgotPassword
    }
})
