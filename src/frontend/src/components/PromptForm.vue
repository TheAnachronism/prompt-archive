<template>
    <form @submit.prevent="handleSubmit" class="space-y-6">
        <div class="space-y-2">
            <Label for="title">Title</Label>
            <Input id="title" v-model="form.title" required />
        </div>

        <div class="space-y-2">
            <Label for="description">Description</Label>
            <Textarea id="description" v-model="form.description" rows="3" />
        </div>

        <div class="space-y-2">
            <Label for="content">Prompt Content</Label>
            <Textarea id="content" v-model="form.promptContent" rows="6" placeholder="Enter your prompt text here..." required
                :disabled="isEditMode" />
            <p v-if="isEditMode" class="text-sm text-muted-foreground">
                To update the prompt content, create a new version after saving these changes.
            </p>
        </div>

        <!-- Models Selection with Autocomplete (Required) -->
        <div class="space-y-2">
            <div class="flex justify-between items-center">
                <Label for="models">AI Models</Label>
                <span v-if="modelError" class="text-sm text-destructive">{{ modelError }}</span>
            </div>
            <div class="flex flex-wrap gap-2">
                <Badge v-for="model in form.models" :key="model" variant="secondary" class="flex items-center gap-1">
                    {{ model }}
                    <Button variant="ghost" size="icon" class="h-4 w-4 rounded-full" @click="removeModel(model)">
                        <XIcon class="h-3 w-3" />
                    </Button>
                </Badge>

                <div class="relative">
                    <Combobox v-model="selectedModel">
                        <div class="flex gap-2">
                            <ComboboxAnchor>
                                <ComboboxInput id="model-input" v-model="modelInput" placeholder="Add model..."
                                    class="w-40" @keydown.enter.prevent="addModel" />
                            </ComboboxAnchor>
                            <Button type="button" variant="outline" size="sm" @click="addModel">
                                <PlusIcon class="h-4 w-4 mr-1" />
                                Add
                            </Button>
                        </div>

                        <ComboboxList class="w-full">
                            <ComboboxEmpty>No models found</ComboboxEmpty>

                            <ComboboxItem v-for="model in filteredModels" :key="model" :value="model">
                                {{ model }}
                                <ComboboxItemIndicator>
                                    <Check class="ml-auto h-4 w-4" />
                                </ComboboxItemIndicator>
                            </ComboboxItem>
                        </ComboboxList>
                    </Combobox>
                </div>
            </div>
        </div>

        <!-- Tags Selection with Autocomplete -->
        <div class="space-y-2">
            <Label for="tags">Tags</Label>
            <div class="flex flex-wrap gap-2">
                <Badge v-for="tag in form.tags" :key="tag" variant="secondary" class="flex items-center gap-1">
                    {{ tag }}
                    <Button variant="ghost" size="icon" class="h-4 w-4 rounded-full" @click="removeTag(tag)">
                        <XIcon class="h-3 w-3" />
                    </Button>
                </Badge>

                <div class="relative">
                    <Combobox v-model="selectedTag">
                        <div class="flex gap-2">
                            <ComboboxAnchor>
                                <ComboboxInput id="tag-input" v-model="tagInput" placeholder="Add tag..." class="w-40"
                                    @keydown.enter.prevent="addTag" />
                            </ComboboxAnchor>
                            <Button type="button" variant="outline" size="sm" @click="addTag">
                                <PlusIcon class="h-4 w-4 mr-1" />
                                Add
                            </Button>
                        </div>

                        <ComboboxList class="w-full">
                            <ComboboxEmpty>No tags found</ComboboxEmpty>

                            <ComboboxItem v-for="tag in filteredTags" :key="tag" :value="tag">
                                {{ tag }}
                                <ComboboxItemIndicator>
                                    <Check class="ml-auto h-4 w-4" />
                                </ComboboxItemIndicator>
                            </ComboboxItem>
                        </ComboboxList>
                    </Combobox>
                </div>
            </div>
        </div>

        <div class="flex justify-end gap-2">
            <Button type="button" variant="outline" @click="$emit('cancel')">
                Cancel
            </Button>
            <Button type="submit" :disabled="isLoading">
                {{ isEditMode ? 'Update' : 'Create' }}
            </Button>
        </div>
    </form>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import {
    Combobox,
    ComboboxAnchor,
    ComboboxInput,
    ComboboxList,
    ComboboxEmpty,
    ComboboxItem,
    ComboboxItemIndicator
} from '@/components/ui/combobox';
import { XIcon, PlusIcon, Check } from 'lucide-vue-next';
import { type Prompt } from '@/utils/promptService';
import { usePromptStore } from '@/store/promptStore';

const props = defineProps<{
    initialData?: Prompt;
    isLoading?: boolean;
}>();

const emit = defineEmits<{
    (e: 'submit', data: { title: string; description: string; content: string; tags: string[]; models: string[] }): void;
    (e: 'cancel'): void;
}>();

const promptStore = usePromptStore();

const isEditMode = !!props.initialData;
const tagInput = ref('');
const modelInput = ref('');
const selectedTag = ref('');
const selectedModel = ref('');
const modelError = ref('');

const form = reactive({
    title: props.initialData?.title || '',
    description: props.initialData?.description || '',
    promptContent: props.initialData?.latestVersion?.promptContent || '',
    tags: props.initialData?.tags || [],
    models: props.initialData?.models || []
});

// Available tags and models for autocomplete
const availableTags = ref<string[]>([]);
const availableModels = ref<string[]>([]);

// Filtered tags and models based on input
const filteredTags = computed(() => {
    if (!tagInput.value) return availableTags.value;
    const lowerInput = tagInput.value.toLowerCase();
    return availableTags.value
        .filter(tag => tag.toLowerCase().includes(lowerInput))
        .filter(tag => !form.tags.includes(tag));
});

const filteredModels = computed(() => {
    if (!modelInput.value) return availableModels.value;
    const lowerInput = modelInput.value.toLowerCase();
    return availableModels.value
        .filter(model => model.toLowerCase().includes(lowerInput))
        .filter(model => !form.models.includes(model));
});

// Watch for selected tag/model from combobox
watch(selectedTag, (newTag) => {
    if (newTag && !form.tags.includes(newTag)) {
        form.tags.push(newTag);
        tagInput.value = '';
        selectedTag.value = '';
    }
});

watch(selectedModel, (newModel) => {
    if (newModel && !form.models.includes(newModel)) {
        form.models.push(newModel);
        modelInput.value = '';
        selectedModel.value = '';
        modelError.value = '';
    }
});

onMounted(async () => {
    // Fetch available tags and models for autocomplete
    try {
        const tags = await promptStore.fetchTags();
        availableTags.value = tags.map(tag => tag.name);

        const models = await promptStore.fetchModels();
        availableModels.value = models.map(model => model.name);
    } catch (error) {
        console.error('Failed to fetch tags or models:', error);
    }
});

function addTag() {
    if (tagInput.value.trim() && !form.tags.includes(tagInput.value.trim())) {
        form.tags.push(tagInput.value.trim());
    }
    tagInput.value = '';
    selectedTag.value = '';
}

function removeTag(tag: string) {
    form.tags = form.tags.filter(t => t !== tag);
}

function addModel() {
    if (modelInput.value.trim() && !form.models.includes(modelInput.value.trim())) {
        form.models.push(modelInput.value.trim());
        modelError.value = '';
    }
    modelInput.value = '';
    selectedModel.value = '';
}

function removeModel(model: string) {
    form.models = form.models.filter(m => m !== model);
}

function handleSubmit() {
    // Validate that at least one model is selected
    if (form.models.length === 0) {
        modelError.value = 'At least one AI model is required';
        return;
    }

    emit('submit', { ...form });
}
</script>