<template>
    <div class="space-y-4">
        <div class="flex justify-between items-center">
            <h3 class="text-lg font-medium">Version Comparison</h3>
            <Button variant="outline" size="sm" @click="$emit('close')">
                <XIcon class="h-4 w-4 mr-1" />
                Close Comparison
            </Button>
        </div>

        <div class="grid grid-cols-2 gap-4">
            <div class="space-y-2">
                <div class="flex items-center">
                    <Badge variant="outline">Version {{ oldVersion.versionNumber }}</Badge>
                    <span class="ml-2 text-sm text-muted-foreground">
                        {{ formatDate(oldVersion.createdAt) }}
                    </span>
                </div>
                <div class="bg-muted p-4 rounded-lg h-full">
                    <pre class="whitespace-pre-wrap text-sm"><code>{{ oldVersion.promptContent }}</code></pre>
                </div>
            </div>

            <div class="space-y-2">
                <div class="flex items-center">
                    <Badge variant="outline">Version {{ newVersion.versionNumber }}</Badge>
                    <span class="ml-2 text-sm text-muted-foreground">
                        {{ formatDate(newVersion.createdAt) }}
                    </span>
                </div>
                <div class="bg-muted p-4 rounded-lg h-full">
                    <div v-for="(line, index) in diffLines" :key="index" :class="{
                        'bg-green-100 dark:bg-green-900/30': line.added,
                        'bg-red-100 dark:bg-red-900/30': line.removed
                    }" class="whitespace-pre-wrap text-sm py-0.5">
                        <span v-if="line.added" class="text-green-600 dark:text-green-400 mr-1">+</span>
                        <span v-if="line.removed" class="text-red-600 dark:text-red-400 mr-1">-</span>
                        {{ line.value }}
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { XIcon } from 'lucide-vue-next';
import { type PromptVersion } from '@/utils/promptService';
import { createDiff } from '@/utils/diffUtils';

const props = defineProps<{
    oldVersion: PromptVersion;
    newVersion: PromptVersion;
}>();

defineEmits<{
    (e: 'close'): void;
}>();

const diffLines = computed(() => {
    return createDiff(props.oldVersion.promptContent, props.newVersion.promptContent);
});

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
}
</script>