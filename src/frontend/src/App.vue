<template>
  <RouterView v-if="!isLoading" />
  <div v-else class="min-h-screen flex items-center justify-center">
    <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-600"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { RouterView } from 'vue-router';
import { useAuth } from '@/utils/authService';

const { checkAuthStatus, isLoading } = useAuth();

onMounted(async () => {
  await checkAuthStatus();
});
</script>