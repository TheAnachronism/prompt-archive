<template>
    <div class="space-y-8">
        <div class="flex flex-col md:flex-row gap-6">
            <!-- Sidebar with filters -->
            <div class="w-full md:w-64 space-y-6">
                <Card>
                    <CardHeader>
                        <CardTitle>Filters</CardTitle>
                    </CardHeader>
                    <CardContent class="space-y-6">
                        <div class="space-y-2">
                            <h3 class="text-sm font-medium">Tags</h3>
                            <div class="space-y-1 max-h-60 overflow-y-auto">
                                <Button v-for="tag in tags" :key="tag.id" variant="ghost"
                                    class="w-full justify-start text-sm"
                                    :class="{ 'bg-accent': selectedTag === tag.name }" @click="selectTag(tag.name)">
                                    <span>{{ tag.name }}</span>
                                    <Badge variant="secondary" class="ml-auto">{{ tag.promptCount }}</Badge>
                                </Button>
                            </div>
                        </div>

                        <div v-if="isAuthenticated" class="space-y-2">
                            <h3 class="text-sm font-medium">View</h3>
                            <Button variant="outline" class="w-full" :class="{ 'bg-accent': showMyPrompts }"
                                @click="toggleMyPrompts">
                                {{ showMyPrompts ? 'Show All Prompts' : 'Show My Prompts' }}
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </div>

            <!-- Main content -->
            <div class="flex-1 space-y-6">
                <div class="flex justify-between items-center">
                    <h1 class="text-2xl font-bold">
                        {{ showMyPrompts ? 'My Prompts' : 'Browse Prompts' }}
                        <span v-if="selectedTag" class="text-lg font-normal ml-2">
                            tagged with <Badge>{{ selectedTag }}</Badge>
                        </span>
                    </h1>

                    <Button v-if="isAuthenticated" @click="router.push('/prompts/create')">
                        Create Prompt
                    </Button>
                </div>

                <div v-if="isLoading" class="flex justify-center py-12">
                    <Spinner class="h-8 w-8" />
                </div>

                <div v-else-if="prompts.length === 0" class="text-center py-12">
                    <div class="text-muted-foreground">No prompts found</div>
                </div>

                <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <PromptCard v-for="prompt in prompts" :key="prompt.id" :prompt="prompt" />
                </div>

                <!-- Pagination -->
                <div v-if="totalCount > 0" class="flex justify-center gap-2 pt-4">
                    <Button variant="outline" size="sm" :disabled="!hasPreviousPage"
                        @click="changePage(currentPage - 1)">
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
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import PromptCard from '@/components/PromptCard.vue';
import Spinner from '@/components/Spinner.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';

const router = useRouter();
const route = useRoute();
const promptStore = usePromptStore();
const authStore = useAuthStore();

const selectedTag = ref('');
const showMyPrompts = ref(false);

const isAuthenticated = computed(() => authStore.isAuthenticated);
const prompts = computed(() => promptStore.prompts);
const tags = computed(() => promptStore.tags);
const isLoading = computed(() => promptStore.isLoading);
const totalCount = computed(() => promptStore.totalCount);
const currentPage = computed(() => promptStore.currentPage);
const pageSize = computed(() => promptStore.pageSize);
const hasNextPage = computed(() => promptStore.hasNextPage);
const hasPreviousPage = computed(() => promptStore.hasPreviousPage);

onMounted(async () => {
    // Parse query parameters
    const page = parseInt(route.query.page as string) || 1;
    const search = route.query.search as string || '';
    const tag = route.query.tag as string || '';
    const userId = route.query.userId as string || '';

    selectedTag.value = tag;
    showMyPrompts.value = !!userId && userId === authStore.user?.id;

    // Load tags
    await promptStore.fetchTags();

    // Load prompts with filters
    await loadPrompts(page, search);
});

watch(route, (newRoute) => {
    const page = parseInt(newRoute.query.page as string) || 1;
    const search = newRoute.query.search as string || '';
    loadPrompts(page, search);
});

async function loadPrompts(page = 1, search = '') {
    const userId = showMyPrompts.value ? authStore.user?.id : undefined;
    await promptStore.fetchPrompts(
        page,
        pageSize.value,
        search,
        selectedTag.value,
        userId
    );
}

function selectTag(tag: string) {
    if (selectedTag.value === tag) {
        selectedTag.value = '';
    } else {
        selectedTag.value = tag;
    }
    applyFilters();
}

function toggleMyPrompts() {
    showMyPrompts.value = !showMyPrompts.value;
    applyFilters();
}

function applyFilters() {
    const query: Record<string, string> = { page: '1' };

    if (route.query.search) {
        query.search = route.query.search as string;
    }

    if (selectedTag.value) {
        query.tag = selectedTag.value;
    }

    if (showMyPrompts.value && authStore.user) {
        query.userId = authStore.user.id;
    }

    router.push({ path: '/prompts', query });
}

function changePage(page: number) {
    router.push({
        path: '/prompts',
        query: {
            ...route.query,
            page: page.toString()
        }
    });
}
</script>