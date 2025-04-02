<template>
    <Card class="h-full flex flex-col">
        <CardHeader>
            <CardTitle class="line-clamp-1">
                <router-link :to="`/prompts/${prompt.id}`" class="hover:underline">
                    {{ prompt.title }}
                </router-link>
            </CardTitle>
            <CardDescription class="flex justify-between items-center">
                <span>By {{ prompt.userName }}</span>
                <span>{{ formatDate(prompt.updatedAt) }}</span>
            </CardDescription>
        </CardHeader>
        <CardContent class="flex-grow">
            <div v-if="prompt.latestVersion?.images?.length && prompt.latestVersion?.images?.length > 0" class="mb-4 relative group overflow-visible">
                <img :src="prompt.latestVersion.images[0].thumbnailUrl"
                    :alt="prompt.latestVersion.images[0].caption || prompt.title"
                    class="w-full h-48 object-cover rounded-md" />
                <div class="absolute opacity-0 group-hover:opacity-100 transition-opacity duration-200 z-30 pointer-events-none flex justify-center items-center"
                    style="inset: -10rem;">
                    <div class="absolute bg-black/90 backdrop-blur-sm rounded-lg shadow-xl overflow-hidden">
                        <img :src="prompt.latestVersion.images[0].thumbnailUrl" :alt="prompt.latestVersion.images[0].caption || prompt.latestVersion.images[0].originalFileName"
                            class="w-full h-full object-contain p-4" />
                        <div class="absolute bottom-0 left-0 right-0 bg-black/80 p-2 text-white">
                            <p class="truncate text-sm">{{ prompt.latestVersion.images[0].originalFileName }}</p>
                            <p v-if="prompt.latestVersion.images[0].caption" class="text-xs opacity-80 truncate">{{ prompt.latestVersion.images[0].caption }}</p>
                        </div>
                    </div>
                </div>
            </div>

            <p class="text-sm text-muted-foreground line-clamp-3 mb-4">
                {{ prompt.description }}
            </p>

            <div v-if="prompt.latestVersion?.promptContent" class="bg-muted p-2 rounded text-sm">
                <p class="line-clamp-3 font-mono text-xs">
                    {{ prompt.latestVersion.promptContent }}
                </p>
            </div>

            <div class="flex flex-wrap gap-1 mt-2">
                <Badge v-for="model in prompt.models" :key="model" variant="default" class="text-xs">
                    {{ model }}
                </Badge>
            </div>

            <div class="flex flex-wrap gap-1 mt-2">
                <Badge v-for="tag in prompt.tags" :key="tag" variant="secondary" class="text-xs">
                    {{ tag }}
                </Badge>
            </div>
        </CardContent>
        <CardFooter class="border-t pt-4 flex justify-between">
            <Badge variant="outline" class="text-xs">
                {{ prompt.versionCount }} {{ prompt.versionCount === 1 ? 'version' : 'versions' }}
            </Badge>
            <div class="flex gap-2">
                <Badge variant="outline" class="text-xs">
                    {{ prompt.latestVersion?.images?.length || 0 }} {{ (prompt.latestVersion?.images?.length || 0) === 1
                        ? 'image' : 'images' }}
                </Badge>
                <Badge variant="outline" class="text-xs">
                    {{ prompt.commentCount }} {{ prompt.commentCount === 1 ? 'comment' : 'comments' }}
                </Badge>
            </div>
        </CardFooter>
    </Card>
</template>

<script setup lang="ts">
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { type Prompt } from '@/utils/promptService';

defineProps<{
    prompt: Prompt;
}>();

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString();
}
</script>