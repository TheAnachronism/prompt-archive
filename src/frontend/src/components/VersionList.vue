<template>
    <div class="space-y-4">
        <div v-if="versions.length === 0" class="text-center py-6 text-muted-foreground">
            No versions available.
        </div>

        <div v-for="version in versions" :key="version.id" class="border rounded-lg p-4 space-y-3">
            <div class="flex justify-between items-start">
                <div>
                    <Badge variant="outline">Version {{ version.versionNumber }}</Badge>
                    <span class="ml-2 text-sm text-muted-foreground">
                        by {{ version.userName }} on {{ formatDate(version.createdAt) }}
                    </span>
                </div>

                <Button variant="ghost" size="sm" @click="$emit('select', version.id)"
                    :class="{ 'bg-accent': version.id === activeVersionId }">
                    {{ version.id === activeVersionId ? 'Hide' : 'Show' }}
                </Button>
            </div>

            <!-- Always show a preview of the content -->
            <div v-if="version.id === activeVersionId" class="mt-4 space-y-4">
                <div class="bg-muted p-4 rounded-lg whitespace-pre-wrap">
                    {{ version.promptContent }}
                </div>

                <!-- Display images if available -->
                <div v-if="version.images?.length" class="mt-4">
                    <h4 class="text-sm font-medium mb-2">Images</h4>
                    <ImageGallery :images="version.images" :can-delete="canEdit"
                        @delete="(imageId: string) => $emit('delete-image', imageId, version.promptId)" />
                </div>

                <div v-if="canEdit" class="flex justify-end">
                    <Button v-if="version.versionNumber > 1" variant="destructive" size="sm" @click="$emit('delete-version', version.id)">
                        <PlusIcon class="h-4 w-4 mr-1" />
                        Delete Version
                    </Button>
                    <Button variant="outline" size="sm" @click="$emit('add-images', version.id)">
                        <PlusIcon class="h-4 w-4 mr-1" />
                        Add Images
                    </Button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import ImageGallery from './prompt/ImageGallery.vue';
import { PlusIcon } from 'lucide-vue-next';
import { type PromptVersion } from '@/utils/promptService';

defineProps<{
    versions: PromptVersion[];
    activeVersionId?: string;
    canEdit?: boolean;
}>();

defineEmits<{
    (e: 'select', versionId: string): void;
    (e: 'add-images', versionId: string): void;
    (e: 'delete-image', imageId: string, promptId: string): void;
    (e: 'delete-version', versionId: string): void;
}>();

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
}
</script>