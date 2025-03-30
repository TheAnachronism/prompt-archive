<template>
  <div class="space-y-8">
    <div class="flex items-center gap-2 text-sm text-muted-foreground mb-2">
      <Button variant="ghost" size="sm" class="p-0 h-auto" @click="router.push('/prompts')">
        Prompts
      </Button>
      <span>/</span>
      <span>Create</span>
    </div>

    <h1 class="text-3xl font-bold">Create Prompt</h1>

    <Card>
      <CardContent class="pt-6">
        <PromptForm :is-loading="isLoading" @submit="createPrompt" @cancel="router.push('/prompts')" />
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import PromptForm from '@/components/PromptForm.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';
import { useToast } from '@/components/ui/toast';

const router = useRouter();
const promptStore = usePromptStore();
const authStore = useAuthStore();
const { toast } = useToast();

const isLoading = computed(() => promptStore.isLoading);
const isAuthenticated = computed(() => authStore.isAuthenticated);

// Redirect if not authenticated
if (!isAuthenticated.value) {
  router.push('/login');
}

async function createPrompt(data: { title: string; description: string; promptContent: string; tags: string[]; models: string[] }) {
  try {
    const newPrompt = await promptStore.createPrompt(data);
    toast({
      title: "Prompt created",
      description: "Your prompt has been created successfully."
    });
    router.push(`/prompts/${newPrompt.id}`);
  } catch (error) {
    toast({
      title: "Error",
      description: "Failed to create prompt. Please try again.",
      variant: "destructive"
    });
    console.error('Failed to create prompt:', error);
  }
}
</script>
