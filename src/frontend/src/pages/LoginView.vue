
<template>
    <div class="flex min-h-full flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div class="sm:mx-auto sm:w-full sm:max-w-md">
        <h2 class="mt-6 text-center text-3xl font-bold tracking-tight">
          Sign in to your account
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
              <div class="flex items-center justify-between">
                <Label for="password">Password</Label>
                <router-link
                  to="/forgot-password"
                  class="text-sm font-medium text-primary hover:text-primary/90"
                >
                  Forgot your password?
                </router-link>
              </div>
              <div class="mt-2">
                <Input
                  id="password"
                  v-model="form.password"
                  name="password"
                  type="password"
                  autocomplete="current-password"
                  required
                  :class="{ 'border-destructive': errors.password }"
                />
                <p v-if="errors.password" class="mt-1 text-sm text-destructive">
                  {{ errors.password }}
                </p>
              </div>
            </div>
  
            <div class="flex items-center">
              <Checkbox id="remember-me" v-model="form.rememberMe" />
              <label
                for="remember-me"
                class="ml-2 block text-sm text-muted-foreground"
              >
                Remember me
              </label>
            </div>
  
            <div>
              <Button
                type="submit"
                class="w-full"
                :disabled="isLoading"
              >
                <Loader2 v-if="isLoading" class="mr-2 h-4 w-4 animate-spin" />
                Sign in
              </Button>
            </div>
          </form>
  
          <div class="mt-6">
            <div class="relative">
              <div
                class="absolute inset-0 flex items-center"
                aria-hidden="true"
              >
                <div class="w-full border-t border-border"></div>
              </div>
              <div class="relative flex justify-center text-sm">
                <span class="bg-card px-2 text-muted-foreground">
                  Or continue with
                </span>
              </div>
            </div>
  
            <div class="mt-6 grid grid-cols-1 gap-3">
              <router-link to="/register" class="text-center">
                <Button variant="outline" class="w-full">
                  Create new account
                </Button>
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, reactive } from 'vue'
  import { useRouter } from 'vue-router'
  import { useToast } from '@/components/ui/toast'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Label } from '@/components/ui/label'
  import { Checkbox } from '@/components/ui/checkbox'
  import { Loader2 } from 'lucide-vue-next'
  import { useAuthStore } from '@/store/auth'
  
  const router = useRouter()
  const { toast } = useToast()
  const authStore = useAuthStore()
  const isLoading = ref(false)
  
  const form = reactive({
    email: '',
    password: '',
    rememberMe: false
  })
  
  const errors = reactive({
    email: '',
    password: '',
    general: ''
  })
  
  const handleSubmit = async () => {
    // Reset errors
    errors.email = ''
    errors.password = ''
    errors.general = ''
  
    // Basic validation
    if (!form.email) {
      errors.email = 'Email is required'
      return
    }
    if (!form.password) {
      errors.password = 'Password is required'
      return
    }
  
    isLoading.value = true
  
    try {
      const success = await authStore.login({
        email: form.email,
        password: form.password,
        rememberMe: form.rememberMe
      })
  
      if (success) {
        toast({
          title: 'Success',
          description: 'You have been logged in successfully',
          variant: 'default'
        })
        router.push('/')
      } else {
        errors.general = 'Invalid email or password'
        toast({
          title: 'Error',
          description: 'Invalid email or password',
          variant: 'destructive'
        })
      }
    } catch (error: any) {
      errors.general = error.message || 'An error occurred during login'
      toast({
        title: 'Error',
        description: errors.general,
        variant: 'destructive'
      })
    } finally {
      isLoading.value = false
    }
  }
  </script>
  