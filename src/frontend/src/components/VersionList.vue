<template>
    <div class="space-y-4">
        <div v-if="versions.length === 0" class="text-center py-6 text-muted-foreground">
            No versions available.
        </div>

        <div v-if="versions.length > 1" class="flex justify-between items-center mb-4">
            <div class="text-sm text-muted-foreground">
                {{ compareMode ? 'Select versions to compare' : '' }}
            </div>
            <Button variant="outline" size="sm" @click="toggleCompareMode">
                <GitCompareIcon class="h-4 w-4 mr-1" />
                {{ compareMode ? 'Cancel' : 'Compare Versions' }}
            </Button>
        </div>

        <div v-if="compareMode && selectedVersions.length === 2" class="mb-4">
            <Button @click="compareVersions">
                <GitCompareIcon class="h-4 w-4 mr-1" />
                Compare Selected Versions
            </Button>
        </div>


        <div v-for="version in versions" :key="version.id" class="border rounded-lg p-4 space-y-3">
            <div class="flex justify-between items-start">
                <div class="flex items-center">
                    <Checkbox v-if="compareMode" :checked="isVersionSelected(version.id)"
                        @update:model-value="toggleVersionSelection(version.id)" class="mr-2" />
                    <div>
                        <Badge variant="outline">Version {{ version.versionNumber }}</Badge>
                        <span class="ml-2 text-sm text-muted-foreground">
                            by {{ version.userName }} on {{ formatDate(version.createdAt) }}
                        </span>
                    </div>
                </div>

                <Button v-if="!compareMode" variant="ghost" size="sm" @click="$emit('select', version.id)"
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
                        @delete="(imageId: string) => $emit('delete-image', imageId, version.promptId)" @set-thumbnail="(imageId: string) => $emit('set-thumbnail', imageId, version.promptId)" />
                </div>

                <div v-if="canEdit" class="flex justify-end">
                    <Button v-if="version.versionNumber > 1" variant="destructive" size="sm"
                        @click="$emit('delete-version', version.id)">
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
import { ref } from 'vue';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from './ui/checkbox';
import ImageGallery from './prompt/ImageGallery.vue';
import { PlusIcon, GitCompareIcon } from 'lucide-vue-next';
import { type PromptVersion } from '@/utils/promptService';

const props = defineProps<{
    versions: PromptVersion[];
    activeVersionId?: string;
    canEdit?: boolean;
}>();

const emit = defineEmits<{
    (e: 'select', versionId: string): void;
    (e: 'add-images', versionId: string): void;
    (e: 'delete-image', imageId: string, promptId: string): void;
    (e: 'delete-version', versionId: string): void;
    (e: 'compare', oldVersionId: string, newVersionId: string): void;
    (e: 'set-thumbnail', imageId: string, promptId: string): void;
}>();

const compareMode = ref(false);
const selectedVersions = ref<string[]>([]);

function toggleCompareMode() {
    compareMode.value = !compareMode.value;
    selectedVersions.value = [];
}

function isVersionSelected(versionId: string): boolean {
    return selectedVersions.value.includes(versionId);
}

function toggleVersionSelection(versionId: string) {
    if (isVersionSelected(versionId)) {
        selectedVersions.value = selectedVersions.value.filter(id => id !== versionId);
    } else {
        if (selectedVersions.value.length < 2) {
            selectedVersions.value.push(versionId);
        } else {
            // Replace the oldest selection
            selectedVersions.value.shift();
            selectedVersions.value.push(versionId);
        }
    }
}

function compareVersions() {
    if (selectedVersions.value.length === 2) {
        // Sort by version number to ensure older version is first
        const versionsToCompare = props.versions
            .filter(v => selectedVersions.value.includes(v.id))
            .sort((a, b) => a.versionNumber - b.versionNumber);

        if (versionsToCompare.length === 2) {
            emit('compare', versionsToCompare[0].id, versionsToCompare[1].id);
        }
    }
}

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
}
</script>