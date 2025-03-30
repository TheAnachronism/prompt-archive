<template>
    <form @submit.prevent="handleSubmit" class="space-y-6">
        <div class="space-y-2">
            <Label for="content">New Version Content</Label>
            <Textarea id="content" v-model="form.promptContent" rows="8"
                placeholder="Enter your updated prompt text here..." required />
        </div>

        <div class="space-y-2">
            <ImageUploader v-model="form.images" v-model:captionsValue="form.imageCaptions" />
        </div>

        <div class="flex justify-end gap-2">
            <Button type="button" variant="outline" @click="$emit('cancel')">
                Cancel
            </Button>
            <Button type="submit" :disabled="isLoading">
                Create Version
            </Button>
        </div>
    </form>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import ImageUploader from './prompt/ImageUploader.vue';

const props = defineProps<{
    initialContent?: string;
    isLoading?: boolean;
}>();

const emit = defineEmits<{
    (e: 'submit', version: { promptContent: string, images: File[], imageCaptions: Record<string, string> }): void;
    (e: 'cancel'): void;
}>();

const form = reactive({
    promptContent: props.initialContent || '',
    images: [] as File[],
    imageCaptions: {} as Record<string, string>
});


function handleSubmit() {
    emit('submit', form);
}
</script>