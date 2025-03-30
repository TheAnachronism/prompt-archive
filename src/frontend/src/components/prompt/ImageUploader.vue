<template>
    <div class="space-y-4">
        <div class="flex items-center justify-between">
            <Label>Images (optional)</Label>
            <p class="text-xs text-muted-foreground">
                {{ modelValue.length }} file(s) selected
            </p>
        </div>

        <div class="border border-dashed rounded-md p-6 flex flex-col items-center justify-center cursor-pointer"
            @dragover.prevent @drop.prevent="onDrop" @click="triggerFileInput">
            <input ref="fileInput" type="file" multiple accept="image/*" class="hidden" @change="onFileChange" />
            <div class="flex flex-col items-center gap-2">
                <UploadIcon class="h-10 w-10 text-muted-foreground" />
                <p class="text-sm text-muted-foreground">
                    Drag and drop images here or click to browse
                </p>
            </div>
        </div>

        <div v-if="modelValue.length > 0" class="space-y-4">
            <div v-for="(file, index) in modelValue" :key="index" class="flex items-center gap-4 p-2 border rounded-md">
                <img :src="previewUrls[index]" alt="Preview" class="h-16 w-16 object-cover rounded-md" />
                <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium truncate">{{ file.name }}</p>
                    <p class="text-xs text-muted-foreground">
                        {{ formatFileSize(file.size) }}
                    </p>
                    <Input v-model="captions[file.name]" placeholder="Add a caption (optional)" class="mt-1 text-xs" />
                </div>
                <Button variant="ghost" size="icon" @click="removeFile(index)" class="text-destructive">
                    <XIcon class="h-4 w-4" />
                </Button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { UploadIcon, XIcon } from 'lucide-vue-next';

const props = defineProps<{
    modelValue: File[];
    captionsValue: Record<string, string>;
}>();

const emit = defineEmits<{
    (e: 'update:modelValue', value: File[]): void;
    (e: 'update:captionsValue', value: Record<string, string>): void;
}>();

const captions = ref<Record<string, string>>(props.captionsValue || {});
const previewUrls = ref<string[]>([]);
const fileInput = ref<HTMLInputElement | null>(null);

watch(() => props.modelValue, (files) => {
    // Generate preview URLs
    previewUrls.value = [];
    files.forEach((file) => {
        const url = URL.createObjectURL(file);
        previewUrls.value.push(url);
    });

    // Clean up old URLs on component unmount
    return () => {
        previewUrls.value.forEach((url) => URL.revokeObjectURL(url));
    };
}, { immediate: true });

watch(captions, (value) => {
    emit('update:captionsValue', value);
}, { deep: true });

function onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
        const newFiles = Array.from(input.files);
        const updatedFiles = [...props.modelValue, ...newFiles];
        emit('update:modelValue', updatedFiles);
    }
}

function onDrop(event: DragEvent) {
    if (event.dataTransfer?.files) {
        const newFiles = Array.from(event.dataTransfer.files).filter(file =>
            file.type.startsWith('image/')
        );
        const updatedFiles = [...props.modelValue, ...newFiles];
        emit('update:modelValue', updatedFiles);
    }
}

function removeFile(index: number) {
    const file = props.modelValue[index];
    if (file && captions.value[file.name]) {
        const newCaptions = { ...captions.value };
        delete newCaptions[file.name];
        captions.value = newCaptions;
    }

    const newFiles = [...props.modelValue];
    newFiles.splice(index, 1);
    emit('update:modelValue', newFiles);
}

function formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' bytes';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}

function triggerFileInput() {
    fileInput.value?.click();
}
</script>