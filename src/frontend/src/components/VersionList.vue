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

                <Button v-if="version.id !== activeVersionId" variant="outline" size="sm"
                    @click="$emit('select', version.id)">
                    View
                </Button>
            </div>

            <div v-if="version.id === activeVersionId" class="bg-muted p-3 rounded whitespace-pre-wrap">
                {{ version.content }}
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { type PromptVersion } from '@/utils/promptService';

defineProps<{
    versions: PromptVersion[];
    activeVersionId?: string;
}>();

defineEmits<{
    (e: 'select', versionId: string): void;
}>();

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
}
</script>