<template>
  <div class="flex min-h-full flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <h2 class="mt-6 text-center text-3xl font-bold tracking-tight">
        Create a new account
      </h2>
    </div>

    <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-card px-4 py-8 shadow sm:rounded-lg sm:px-10">
        <form class="space-y-6" @submit.prevent="handleSubmit">
          <div>
            <Label for="email">Email address</Label>
            <div class="mt-2">
              <Input
                id="email"
                v-model="form.email"
                name="email"
                type="email"
                autocomplete="email"
                required
                :class="{ 'border-destructive': errors.email }"
              />
              <p v-if="errors.email" class="mt-1 text-sm text-destructive">
                {{ errors.email }}
              </p>
            </div>
          </div>

          <div>
            <Label for="username">Username</Label>
            <div class="mt-2">
              <Input
                id="username"
                v-model="form.userName"
                name="username"
                type="text"
                autocomplete="username"
                required
                :class="{ 'border-destructive': errors.userName }"
              />
              <p v-if="errors.userName" class="mt-1 text-sm text-destructive">
                {{ errors.userName }}
              </p>
            </div>
          </div>

          <div>
            <Label for="password">Password</Label>
            <div class="mt-2">
              <Input
                id="password"
                v-model="form.password"
                name="password"
                type="password"
                autocomplete="new-password"
                required
                :class="{ 'border-destructive': errors.password }"
              />
              <p v-if="errors.password" class="mt-1 text-sm text-destructive">
                {{ errors.password }}
              </p>
            </div>
          </div>

          <div>
            <Label for="passwordConfirm">Confirm Password</Label>
            <div class="mt-2">
              <Input
                id="passwordConfirm"
                v-model="form.passwordConfirm"
                name="passwordConfirm"
                type="password"
                autocomplete="new-password"
                required
                :class="{ 'border-destructive': errors.passwordConfirm }"
              />
              <p
                v-if="errors.passwordConfirm"
                class="mt-1 text-sm text-destructive"
              >
                {{ errors.passwordConfirm }}
              </p>
            </div>
          </div>

          <div>
            <Button type="submit" class="w-full" :disabled="isLoading">
              <Loader2 v-if="isLoading" class="mr-2 h-4 w-4 animate-spin" />
              Register
            </Button>
          </div>
        </form>

        <div class="mt-6">
          <div class="relative">
            <div class="absolute inset-0 flex items-center" aria-hidden="true">
              <div class="w-full border-t border-border"></div>
            </div>
            <div class="relative flex justify-center text-sm">
              <span class="bg-card px-2 text-muted-foreground">
                Already have an account?
              </span>
            </div>
          </div>

          <div class="mt-6 grid grid-cols-1 gap-3">
            <router-link to="/login" class="text-center">
              <Button variant="outline" class="w-full">Sign in</Button>
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { useToast } from "@/components/ui/toast";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2 } from "lucide-vue-next";
import { useAuthStore } from "@/store/auth";

const router = useRouter();
const { toast } = useToast();
const authStore = useAuthStore();
const isLoading = ref(false);

const form = reactive({
  email: "",
  userName: "",
  password: "",
  passwordConfirm: "",
});

const errors = reactive({
  email: "",
  userName: "",
  password: "",
  passwordConfirm: "",
  general: "",
});

const validateForm = () => {
  let isValid = true;

  // Reset errors
  errors.email = "";
  errors.userName = "";
  errors.password = "";
  errors.passwordConfirm = "";
  errors.general = "";

  // Email validation
  if (!form.email) {
    errors.email = "Email is required";
    isValid = false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = "Please enter a valid email address";
    isValid = false;
  }

  // Username validation
  if (!form.userName) {
    errors.userName = "Username is required";
    isValid = false;
  } else if (form.userName.length < 3) {
    errors.userName = "Username must be at least 3 characters";
    isValid = false;
  }

  // Password validation
  if (!form.password) {
    errors.password = "Password is required";
    isValid = false;
  } else if (form.password.length < 8) {
    errors.password = "Password must be at least 8 characters";
    isValid = false;
  }

  // Confirm password validation
  if (!form.passwordConfirm) {
    errors.passwordConfirm = "Please confirm your password";
    isValid = false;
  } else if (form.password !== form.passwordConfirm) {
    errors.passwordConfirm = "Passwords do not match";
    isValid = false;
  }

  return isValid;
};

const handleSubmit = async () => {
  if (!validateForm()) {
    return;
  }

  isLoading.value = true;

  try {
    const success = await authStore.register(form);

    if (success) {
      toast({
        title: "Success",
        description: "Your account has been created successfully",
        variant: "default",
      });
      router.push("/");
    } else {
      errors.general = "Registration failed";
      toast({
        title: "Error",
        description: "Registration failed. Please try again.",
        variant: "destructive",
      });
    }
  } catch (error: any) {
    errors.general = error.message || "An error occurred during registration";
    toast({
      title: "Error",
      description: errors.general,
      variant: "destructive",
    });
  } finally {
    isLoading.value = false;
  }
};
</script>
