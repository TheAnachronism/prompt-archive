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
                            <h3 class="text-sm font-medium">Models</h3>
                            <AutoCompleteCombo :items="selectedModels" :availableItems="modelNames"
                                placeholder="Filter by tags..." :add-unknown-items="false"
                                :items-added-handler="applyFilters" :hide-add-button="true" />

                            <div class="flex flex-wrap gap-1 mt-2">
                                <Badge v-for="model in selectedModels" :key="model" class="flex items-center gap-1">
                                    {{ model }}
                                    <Button size="icon" class="h-4 w-4 p-0 hover:bg-transparent"
                                        @click="removeModel(model)">
                                        <XIcon class="h-3 w-3" />
                                    </Button>
                                </Badge>
                            </div>

                            <h3 class="text-sm font-medium">Tags</h3>
                            <AutoCompleteCombo :items="selectedTags" :availableItems="tagNames"
                                placeholder="Filter by tags..." :add-unknown-items="false"
                                :items-added-handler="applyFilters" :hide-add-button="true" />

                            <div class="flex flex-wrap gap-1 mt-2">
                                <Badge v-for="tag in selectedTags" :key="tag" variant="secondary"
                                    class="flex items-center gap-1">
                                    {{ tag }}
                                    <Button variant="ghost" size="icon" class="h-4 w-4 p-0 hover:bg-transparent"
                                        @click="removeTag(tag)">
                                        <XIcon class="h-3 w-3" />
                                    </Button>
                                </Badge>
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
                        <span v-if="selectedModels.length > 0" class="text-lg font-normal ml-2">
                            for {{ selectedModels.length > 1 ? 'models' : 'model' }}
                            <Badge v-for="model in selectedModels" :key="model" class="mr-1 inline-flex items-center gap-1">
                                {{ model }}
                                <Button size="icon" class="h-4 w-4 p-0 hover:bg-transparent"
                                    @click="removeModel(model)">
                                    <XIcon class="h-3 w-3" />
                                </Button>
                            </Badge>
                        </span>
                        <span v-if="selectedTags.length > 0" class="text-lg font-normal ml-2">
                            tagged with
                            <Badge v-for="tag in selectedTags" :key="tag" variant="secondary" class="mr-1 inline-flex items-center gap-1">
                                {{ tag }}
                                <Button variant="ghost" size="icon" class="h-4 w-4 p-0 hover:bg-transparent"
                                    @click="removeTag(tag)">
                                    <XIcon class="h-3 w-3" />
                                </Button>
                            </Badge>
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
import AutoCompleteCombo from '@/components/prompt/AutoCompleteCombo.vue';
import Spinner from '@/components/Spinner.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';
import { XIcon } from 'lucide-vue-next';

const router = useRouter();
const route = useRoute();
const promptStore = usePromptStore();
const authStore = useAuthStore();

const selectedTags = ref<string[]>([]);
const selectedModels = ref<string[]>([]);
const showMyPrompts = ref(false);

const isAuthenticated = computed(() => authStore.isAuthenticated);
const prompts = computed(() => promptStore.prompts);
const tags = computed(() => promptStore.tags);
const tagNames = computed(() => tags.value.map(tag => tag.name));
const models = computed(() => promptStore.models);
const modelNames = computed(() => models.value.map(model => model.name));
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
    const modelParam = route.query.models as string || '';
    const tagParam = route.query.tags as string || '';
    const userId = route.query.userId as string || '';

    selectedModels.value = modelParam ? modelParam.split(',') : [];
    selectedTags.value = tagParam ? tagParam.split(',') : [];
    showMyPrompts.value = !!userId && userId === authStore.user?.id;

    // Load tags
    await promptStore.fetchTags();
    await promptStore.fetchModels();

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
        selectedModels.value,
        selectedTags.value,
        userId
    );
}

function toggleMyPrompts() {
    showMyPrompts.value = !showMyPrompts.value;
    applyFilters();
}

function removeTag(tag: string) {
    selectedTags.value = selectedTags.value.filter(t => t !== tag);
    applyFilters();
}

function removeModel(model: string) {
    selectedModels.value = selectedModels.value.filter(m => m !== model);
    applyFilters();
}

function applyFilters() {
    const query: Record<string, string> = { page: '1' };

    if (route.query.search) {
        query.search = route.query.search as string;
    }

    if (selectedModels.value.length > 0) {
        query.models = selectedModels.value.join(',');
    }

    if (selectedTags.value.length > 0) {
        query.tags = selectedTags.value.join(',');
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