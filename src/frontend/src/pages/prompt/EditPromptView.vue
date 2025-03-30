<template>
  <div class="space-y-8">
    <div v-if="isLoading && !prompt" class="flex justify-center py-12">
      <Spinner class="h-8 w-8" />
    </div>

    <template v-else>
      <div class="flex items-center gap-2 text-sm text-muted-foreground mb-2">
        <Button variant="ghost" size="sm" class="p-0 h-auto" @click="router.push('/prompts')">
          Prompts
        </Button>
        <span>/</span>
        <Button variant="ghost" size="sm" class="p-0 h-auto" @click="router.push(`/prompts/${promptId}`)">
          {{ prompt?.title || 'Prompt' }}
        </Button>
        <span>/</span>
        <span>Edit</span>
      </div>

      <h1 class="text-3xl font-bold">Edit Prompt</h1>

      <Card v-if="prompt">
        <CardContent class="pt-6">
          <PromptForm :initial-data="prompt" :is-loading="isLoading" @submit="updatePrompt"
            @cancel="router.push(`/prompts/${promptId}`)" />
        </CardContent>
      </Card>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import PromptForm from '@/components/PromptForm.vue';
import Spinner from '@/components/Spinner.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';
import { useToast } from '@/components/ui/toast';

const router = useRouter();
const route = useRoute();
const promptStore = usePromptStore();
const authStore = useAuthStore();
const { toast } = useToast();

const promptId = computed(() => route.params.id as string);
const prompt = computed(() => promptStore.currentPrompt);
const isLoading = computed(() => promptStore.isLoading);
const isAuthenticated = computed(() => authStore.isAuthenticated);
const currentUserId = computed(() => authStore.user?.id);
const isAdmin = computed(() => authStore.user?.roles?.includes('Admin') ?? false);

// Redirect if not authenticated
if (!isAuthenticated.value) {
  router.push('/login');
}

onMounted(async () => {
  if (promptId.value) {
    await promptStore.fetchPromptById(promptId.value);

    // Check if user has permission to edit
    if (prompt.value && prompt.value.userId !== currentUserId.value && !isAdmin.value) {
      toast({
        title: "Access denied",
        description: "You don't have permission to edit this prompt.",
        variant: "destructive"
      });
      router.push(`/prompts/${promptId.value}`);
    }
  }
});

async function updatePrompt(data: { title: string; description: string; content: string; tags: string[]; models: string[] }) {
  if (!promptId.value) return;

  try {
    await promptStore.updatePrompt(promptId.value, {
      title: data.title,
      description: data.description,
      tags: data.tags,
      models: data.models
    });

    toast({
      title: "Prompt updated",
      description: "Your prompt has been updated successfully."
    });

    router.push(`/prompts/${promptId.value}`);
  } catch (error) {
    toast({
      title: "Error",
      description: "Failed to update prompt. Please try again.",
      variant: "destructive"
    });
    console.error('Failed to update prompt:', error);
  }
}
</script>
