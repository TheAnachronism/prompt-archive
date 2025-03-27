<template>
    <Card>
        <CardHeader>
            <CardTitle>Profile Information</CardTitle>
            <CardDescription>
                Update your account's profile information.
            </CardDescription>
        </CardHeader>
        <CardContent>
            <form @submit.prevent="updateProfile" class="space-y-4">
                <div class="space-y-2">
                    <Label for="username">Username</Label>
                    <Input id="username" v-model="form.userName" placeholder="Your username" />
                    <p v-if="errors.userName" class="text-sm text-destructive">
                        {{ errors.userName }}
                    </p>
                </div>

                <div class="space-y-2">
                    <Label for="email">Email</Label>
                    <Input id="email" type="email" v-model="form.email" placeholder="Your email address" />
                    <p v-if="errors.email" class="text-sm text-destructive">
                        {{ errors.email }}
                    </p>
                </div>

                <div class="flex justify-end">
                    <Button type="submit" :disabled="isLoading">
                        <Loader2 v-if="isLoading" class="mr-2 h-4 w-4 animate-spin" />
                        Save Changes
                    </Button>
                </div>
            </form>
        </CardContent>
    </Card>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
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
    userName: "",
    email: "",
});

const errors = ref({
    userName: "",
    email: "",
});

const isLoading = computed(() => accountStore.isLoading);

// Initialize form with current profile data
watch(
    () => accountStore.profile,
    (profile) => {
        if (profile) {
            form.value.userName = profile.userName;
            form.value.email = profile.email;
        }
    },
    { immediate: true }
);

const validateForm = () => {
    let isValid = true;
    errors.value.userName = "";
    errors.value.email = "";

    if (!form.value.userName.trim()) {
        errors.value.userName = "Username is required";
        isValid = false;
    } else if (form.value.userName.length < 3) {
        errors.value.userName = "Username must be at least 3 characters";
        isValid = false;
    }

    if (!form.value.email.trim()) {
        errors.value.email = "Email is required";
        isValid = false;
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
        errors.value.email = "Please enter a valid email address";
        isValid = false;
    }

    return isValid;
};

const updateProfile = async () => {
    if (!validateForm()) return;

    const success = await accountStore.updateProfile({
        userName: form.value.userName,
        email: form.value.email,
    });

    if (success) {
        toast({
            title: "Profile updated",
            description: "Your profile information has been updated successfully.",
        });
    } else {
        toast({
            title: "Update failed",
            description: accountStore.error || "Failed to update profile information.",
            variant: "destructive",
        });
    }
};
</script>