<template>
    <div class="space-y-4">
        <div v-if="images.length > 0" class="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div v-for="image in images" :key="image.id" class="relative group border rounded-md overflow-visible">
                <img :src="image.thumbnailUrl" :alt="image.caption || image.originalFileName" loading="lazy"
                    class="w-full h-48 object-cover cursor-pointer" />
                <div class="absolute opacity-0 group-hover:opacity-100 transition-opacity duration-200 z-30 pointer-events-none flex justify-center items-center"
                    style="inset: -10rem;">
                    <div class="absolute bg-black/90 backdrop-blur-sm rounded-lg shadow-xl overflow-hidden">
                        <img :src="image.thumbnailUrl" :alt="image.caption || image.originalFileName"
                            class="w-full h-full object-contain p-4" />
                        <div class="absolute bottom-0 left-0 right-0 bg-black/80 p-2 text-white">
                            <p class="truncate text-sm">{{ image.originalFileName }}</p>
                            <p v-if="image.caption" class="text-xs opacity-80 truncate">{{ image.caption }}</p>
                        </div>
                    </div>
                </div>

                <div class="absolute inset-0 cursor-pointer z-20" @click="openLightbox(image)"></div>

            </div>
        </div>
        <div v-else class="text-center py-8 text-muted-foreground">
            No images available
        </div>

        <!-- Lightbox -->
        <Dialog :open="!!selectedImage" @update:open="closeLightbox">
            <DialogContent class="max-w-4xl p-0 overflow-hidden grid grid-cols-4 gap-4">
                <div class="col-start-2 col-span-2">
                    <div class="relative">
                        <img v-if="selectedImage" :src="selectedImage.imageUrl"
                            :alt="selectedImage.caption || selectedImage.originalFileName"
                            class="w-full max-h-[80vh] object-contain p-5" />
                    </div>
                    <div v-if="selectedImage" class="p-4 bg-background flex flex-col items-center">
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
                </div>
                <div class=" p-5 self-end flex flex-col justify-end gap-2">
                    <Button v-if="canDelete"
                        @click.stop="$emit('setThumbnail', selectedImage?.id || '')">
                        Set as Thumbnail
                        <Image class="h-8 w-8" />
                    </Button>
                    <div class="flex justify-end gap-2">
                        <Button v-if="canDelete" variant="destructive"
                            @click.stop="$emit('delete', selectedImage?.id || '')">
                            Delete
                            <TrashIcon class="h-8 w-8" />
                        </Button>
                        <Button variant="default" @click.stop="downloadImage">
                            Download
                            <ImageDown class="h-8 w-8" />
                        </Button>
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
import { TrashIcon, ImageDown, Image } from 'lucide-vue-next';
import type { PromptImage } from '@/utils/promptService';

defineProps<{
    images: PromptImage[];
    canDelete?: boolean;
}>();

defineEmits<{
    (e: 'delete', imageId: string): void;
    (e: 'setThumbnail', imageId: string): void;
}>();

const selectedImage = ref<PromptImage | null>(null);
const isDownloading = ref(false);

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

async function downloadImage() {
    if (!selectedImage.value)
        return;

    isDownloading.value = true;

    try {
        const response = await fetch(selectedImage.value.imageUrl);
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = url;
        link.download = selectedImage.value.originalFileName;
        document.body.appendChild(link);
        link.click();

        // Clean up
        window.URL.revokeObjectURL(url);
        document.body.removeChild(link);
    } catch (error) {
        console.error('Download failed:', error);
    } finally {
        isDownloading.value = false;
    }
}
</script>