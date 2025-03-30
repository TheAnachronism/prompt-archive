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
            <p class="text-sm text-muted-foreground line-clamp-3 mb-4">
                {{ prompt.description }}
            </p>

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
            <Badge variant="outline" class="text-xs">
                {{ prompt.commentCount }} {{ prompt.commentCount === 1 ? 'comment' : 'comments' }}
            </Badge>
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