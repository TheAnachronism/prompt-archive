<template>
    <div class="space-y-8">
        <h1 class="text-3xl font-bold">My Prompts</h1>

        <div v-if="isLoading" class="flex justify-center py-12">
            <Spinner class="h-8 w-8" />
        </div>

        <div v-else-if="prompts.length === 0" class="text-center py-12">
            <div class="text-muted-foreground">You haven't created any prompts yet</div>
            <Button class="mt-4" @click="router.push('/prompts/create')">
                Create Your First Prompt
            </Button>
        </div>

        <div v-else>
            <div class="flex justify-between items-center mb-6">
                <div class="text-muted-foreground">
                    Showing {{ prompts.length }} of {{ totalCount }} prompts
                </div>
                <Button @click="router.push('/prompts/create')">
                    Create Prompt
                </Button>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <PromptCard v-for="prompt in prompts" :key="prompt.id" :prompt="prompt" />
            </div>

            <!-- Pagination -->
            <div v-if="totalCount > 0" class="flex justify-center gap-2 pt-8">
                <Button variant="outline" size="sm" :disabled="!hasPreviousPage" @click="changePage(currentPage - 1)">
                    Previous
                </Button>

                <div class="flex items-center text-sm px-2">
                    Page {{ currentPage }} of {{ Math.ceil(totalCount / pageSize) }}
                </div>

                <Button variant="outline" size="sm" :disabled="!hasNextPage" @click="changePage(currentPage + 1)">
                    Next
                </Button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Button } from '@/components/ui/button';
import PromptCard from '@/components/PromptCard.vue';
import Spinner from '@/components/Spinner.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';

const router = useRouter();
const route = useRoute();
const promptStore = usePromptStore();
const authStore = useAuthStore();

const isAuthenticated = computed(() => authStore.isAuthenticated);
const prompts = computed(() => promptStore.prompts);
const isLoading = computed(() => promptStore.isLoading);
const totalCount = computed(() => promptStore.totalCount);
const currentPage = computed(() => promptStore.currentPage);
const pageSize = computed(() => promptStore.pageSize);
const hasNextPage = computed(() => promptStore.hasNextPage);
const hasPreviousPage = computed(() => promptStore.hasPreviousPage);

// Redirect if not authenticated
if (!isAuthenticated.value) {
    router.push('/login');
}

onMounted(async () => {
    const page = parseInt(route.query.page as string) || 1;
    await loadPrompts(page);
});

async function loadPrompts(page = 1) {
    if (authStore.user) {
        await promptStore.fetchPrompts(page, pageSize.value, undefined, undefined, authStore.user.id);
    }
}

function changePage(page: number) {
    router.push({
        path: '/my-prompts',
        query: { page: page.toString() }
    });
    loadPrompts(page);
}
</script>