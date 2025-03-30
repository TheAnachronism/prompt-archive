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
            <Textarea id="content" v-model="form.promptContent" rows="6" placeholder="Enter your prompt text here..."
                required :disabled="isEditMode" />
            <p v-if="isEditMode" class="text-sm text-muted-foreground">
                To update the prompt content, create a new version after saving these changes.
            </p>
        </div>

        <div class="space-y-4">
            <div class="grid grid-cols-2 gap-6">
                <!-- Models Column -->
                <div class="space-y-4">
                    <div class="flex justify-between items-center">
                        <Label for="models">AI Models</Label>
                        <span v-if="modelError" class="text-sm text-destructive">{{ modelError }}</span>
                    </div>

                    <!-- Input for models -->
                    <div class="relative w-full">
                        <AutoCompleteCombo :items="form.models" :available-items="availableModels"
                            placeholder="Add models..." :set-error="setModelError" :add-unknown-items="true" />
                    </div>

                    <!-- Results for models -->
                    <div class="flex flex-wrap gap-2 mt-2">
                        <Badge v-for="model in form.models" :key="model" variant="secondary"
                            class="flex items-center gap-1 py-1 px-2">
                            {{ model }}
                            <Button variant="ghost" size="icon" class="h-4 w-4 rounded-full"
                                @click="removeModel(model)">
                                <XIcon class="h-3 w-3" />
                            </Button>
                        </Badge>
                    </div>
                </div>

                <!-- Tags Column -->
                <div class="space-y-4">
                    <div class="flex justify-between items-center">
                        <Label for="tags">Tags</Label>
                    </div>
                    <!-- Input for tags -->
                    <div class="relative w-full">
                        <AutoCompleteCombo :items="form.tags" :available-items="availableTags" placeholder="Add tag..."
                            :add-unknown-items="true" />
                    </div>

                    <!-- Results for tags -->
                    <div class="flex flex-wrap gap-2 mt-2">
                        <Badge v-for="tag in form.tags" :key="tag" variant="secondary"
                            class="flex items-center gap-1 py-1 px-2">
                            {{ tag }}
                            <Button variant="ghost" size="icon" class="h-4 w-4 rounded-full" @click="removeTag(tag)">
                                <XIcon class="h-3 w-3" />
                            </Button>
                        </Badge>
                    </div>
                </div>
            </div>
        </div>

        <div class="space-y-2">
            <ImageUploader v-if="!isEditMode" v-model="form.images" v-model:captionsValue="form.imageCaptions" />
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
import { ref, reactive, onMounted } from 'vue';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import AutoCompleteCombo from './prompt/AutoCompleteCombo.vue';
import ImageUploader from './prompt/ImageUploader.vue';
import { XIcon } from 'lucide-vue-next';
import { type Prompt } from '@/utils/promptService';
import { usePromptStore } from '@/store/promptStore';

const props = defineProps<{
    initialData?: Prompt;
    isLoading?: boolean;
}>();

const emit = defineEmits<{
    (e: 'submit', data: { title: string; description: string; promptContent: string; tags: string[]; models: string[] }): void;
    (e: 'cancel'): void;
}>();

const promptStore = usePromptStore();

const isEditMode = !!props.initialData;
const modelError = ref('');

const form = reactive({
    title: props.initialData?.title || '',
    description: props.initialData?.description || '',
    promptContent: props.initialData?.latestVersion?.promptContent || '',
    tags: props.initialData?.tags || [],
    models: props.initialData?.models || [],
    images: [] as File[],
    imageCaptions: {} as Record<string, string>
});

// Available tags and models for autocomplete
const availableTags = ref<string[]>([]);
const availableModels = ref<string[]>([]);

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

function removeTag(tag: string) {
    form.tags = form.tags.filter(t => t !== tag);
}

function setModelError(error: string) {
    modelError.value = error;
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