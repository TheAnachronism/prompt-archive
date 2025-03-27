import { defineStore } from "pinia";
import { ref } from "vue";
import {
    accountService,
    type UserProfile,
    type UpdateProfileRequest,
    type ChangePasswordRequest
} from "@/utils/accountService";

export const useAccountStore = defineStore("account", () => {
    const profile = ref<UserProfile | null>(null);
    const isLoading = ref(false);
    const error = ref<string | null>(null);

    async function fetchProfile() {
        isLoading.value = true;
        error.value = null;
        try {
            profile.value = await accountService.getProfile();
        } catch (err) {
            error.value = "Failed to load profile";
            console.error(err);
        } finally {
            isLoading.value = false;
        }
    }

    async function updateProfile(profileData: UpdateProfileRequest) {
        isLoading.value = true;
        error.value = null;
        try {
            profile.value = await accountService.updateProfile(profileData);
            return true;
        } catch (err) {
            error.value = "Failed to update profile";
            console.error(err);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    async function changePassword(request: ChangePasswordRequest) {
        isLoading.value = true;
        error.value = null;
        try {
            await accountService.changePassword(request);
            return true;
        } catch (err) {
            error.value = "Failed to change password";
            console.error(err);
            return false;
        } finally {
            isLoading.value = false;
        }
    }

    return {
        profile,
        isLoading,
        error,
        fetchProfile,
        updateProfile,
        changePassword
    }
});