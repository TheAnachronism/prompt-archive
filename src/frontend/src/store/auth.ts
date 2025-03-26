import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
    const user = ref(null)
    const isAuthenticated = ref(false)

    function login(credentials: any) {
        // TODO: Implement login logic
        isAuthenticated.value = true
    }

    function logout() {
        user.value = null
        isAuthenticated.value = false
    }

    return { user, isAuthenticated, login, logout }
})
