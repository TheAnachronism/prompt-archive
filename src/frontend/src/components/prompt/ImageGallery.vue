<template>
    <div class="space-y-4">
        <div v-if="images.length > 0" class="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div v-for="image in images" :key="image.id" class="relative group border rounded-md overflow-hidden">
                <img :src="image.imageUrl" :alt="image.caption || image.originalFileName"
                    class="w-full h-48 object-cover cursor-pointer" @click="openLightbox(image)" />
                <div
                    class="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex flex-col justify-between p-2">
                    <div class="flex justify-end">
                        <Button v-if="canDelete" variant="destructive" size="icon" class="h-8 w-8"
                            @click.stop="$emit('delete', image.id)">
                            <TrashIcon class="h-4 w-4" />
                        </Button>
                    </div>
                    <div class="bg-black/70 p-2 text-white text-sm">
                        <p class="truncate">{{ image.originalFileName }}</p>
                        <p v-if="image.caption" class="text-xs opacity-80 truncate">
                            {{ image.caption }}
                        </p>
                    </div>
                </div>
            </div>
        </div>
        <div v-else class="text-center py-8 text-muted-foreground">
            No images available
        </div>

        <!-- Lightbox -->
        <Dialog :open="!!selectedImage" @update:open="closeLightbox">
            <DialogContent class="max-w-4xl p-0 overflow-hidden">
                <div class="relative">
                    <img v-if="selectedImage" :src="selectedImage.imageUrl"
                        :alt="selectedImage.caption || selectedImage.originalFileName"
                        class="w-full max-h-[80vh] object-contain" />
                    <Button variant="ghost" size="icon"
                        class="absolute top-2 right-2 bg-black/50 text-white hover:bg-black/70" @click="closeLightbox">
                        <XIcon class="h-4 w-4" />
                    </Button>
                </div>
                <div v-if="selectedImage" class="p-4 bg-background">
                    <h3 class="font-medium">{{ selectedImage.originalFileName }}</h3>
                    <p v-if="selectedImage.caption" class="text-sm text-muted-foreground">
                        {{ selectedImage.caption }}
                    </p>
                    <div class="flex items-center gap-2 mt-2 text-xs text-muted-foreground">
                        <span>{{ formatFileSize(selectedImage.fileSizeBytes) }}</span>
                        <span>•</span>
                        <span>{{ formatDate(selectedImage.createdAt) }}</span>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent } from '@/components/ui/dialog';
import { TrashIcon, XIcon } from 'lucide-vue-next';
import type { PromptImage } from '@/utils/promptService';

defineProps<{
    images: PromptImage[];
    canDelete?: boolean;
}>();

defineEmits<{
    (e: 'delete', imageId: string): void;
}>();

const selectedImage = ref<PromptImage | null>(null);

function openLightbox(image: PromptImage) {
    selectedImage.value = image;
}

function closeLightbox() {
    selectedImage.value = null;
}

function formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' bytes';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}

function formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
}
</script>