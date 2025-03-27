<template>
    <Card>
        <CardHeader>
            <CardTitle>Change Password</CardTitle>
            <CardDescription>
                Update your password to keep your account secure.
            </CardDescription>
        </CardHeader>
        <CardContent>
            <form @submit.prevent="changePassword" class="space-y-4">
                <div class="space-y-2">
                    <Label for="current-password">Current Password</Label>
                    <Input id="current-password" type="password" v-model="form.currentPassword"
                        placeholder="Your current password" />
                    <p v-if="errors.currentPassword" class="text-sm text-destructive">
                        {{ errors.currentPassword }}
                    </p>
                </div>

                <div class="space-y-2">
                    <Label for="new-password">New Password</Label>
                    <Input id="new-password" type="password" v-model="form.newPassword"
                        placeholder="Your new password" />
                    <p v-if="errors.newPassword" class="text-sm text-destructive">
                        {{ errors.newPassword }}
                    </p>
                </div>

                <div class="space-y-2">
                    <Label for="confirm-password">Confirm New Password</Label>
                    <Input id="confirm-password" type="password" v-model="form.confirmPassword"
                        placeholder="Confirm your new password" />
                    <p v-if="errors.confirmPassword" class="text-sm text-destructive">
                        {{ errors.confirmPassword }}
                    </p>
                </div>

                <div class="flex justify-end">
                    <Button type="submit" :disabled="isLoading">
                        <Loader2 v-if="isLoading" class="mr-2 h-4 w-4 animate-spin" />
                        Change Password
                    </Button>
                </div>
            </form>
        </CardContent>
    </Card>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useAccountStore } from "@/store/account";
import { useToast } from "@/components/ui/toast";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Loader2 } from "lucide-vue-next";

const accountStore = useAccountStore();
const { toast } = useToast();

const form = ref({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
});

const errors = ref({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
});

const isLoading = computed(() => accountStore.isLoading);

const validateForm = () => {
    let isValid = true;
    errors.value.currentPassword = "";
    errors.value.newPassword = "";
    errors.value.confirmPassword = "";

    if (!form.value.currentPassword) {
        errors.value.currentPassword = "Current password is required";
        isValid = false;
    }

    if (!form.value.newPassword) {
        errors.value.newPassword = "New password is required";
        isValid = false;
    } else if (form.value.newPassword.length < 8) {
        errors.value.newPassword = "Password must be at least 8 characters";
        isValid = false;
    } else if (!/[A-Z]/.test(form.value.newPassword)) {
        errors.value.newPassword = "Password must contain at least one uppercase letter";
        isValid = false;
    } else if (!/[a-z]/.test(form.value.newPassword)) {
        errors.value.newPassword = "Password must contain at least one lowercase letter";
        isValid = false;
    } else if (!/[0-9]/.test(form.value.newPassword)) {
        errors.value.newPassword = "Password must contain at least one number";
        isValid = false;
    } else if (!/[^a-zA-Z0-9]/.test(form.value.newPassword)) {
        errors.value.newPassword = "Password must contain at least one special character";
        isValid = false;
    }

    if (form.value.newPassword !== form.value.confirmPassword) {
        errors.value.confirmPassword = "Passwords do not match";
        isValid = false;
    }

    return isValid;
};

const changePassword = async () => {
    if (!validateForm()) return;

    const success = await accountStore.changePassword({
        password: form.value.currentPassword,
        newPassword: form.value.newPassword,
        newPasswordConfirm: form.value.confirmPassword
    });

    if (success) {
        toast({
            title: "Password changed",
            description: "Your password has been changed successfully.",
        });
        // Reset form
        form.value.currentPassword = "";
        form.value.newPassword = "";
        form.value.confirmPassword = "";
    } else {
        toast({
            title: "Password change failed",
            description: accountStore.error || "Failed to change password.",
            variant: "destructive",
        });
    }
};
</script>