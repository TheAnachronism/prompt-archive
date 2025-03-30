<template>
    <div class="space-y-4">
        <div v-if="comments.length === 0" class="text-center py-6 text-muted-foreground">
            No comments yet. Be the first to comment!
        </div>

        <div v-for="comment in comments" :key="comment.id" class="border rounded-lg p-4 space-y-2">
            <div class="flex justify-between items-start">
                <div class="font-medium">@{{ comment.userName }}</div>
                <div class="flex items-center gap-2">
                    <div class="text-xs text-muted-foreground">
                        {{ formatDate(comment.updatedAt || comment.createdAt) }}
                        {{ comment.updatedAt ? '(edited)' : '' }}
                    </div>

                    <DropdownMenu v-if="canModify(comment)">
                        <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="icon" class="h-8 w-8">
                                <MoreVerticalIcon class="h-4 w-4" />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                            <DropdownMenuItem @click="$emit('edit', comment)">
                                <PencilIcon class="mr-2 h-4 w-4" />
                                <span>Edit</span>
                            </DropdownMenuItem>
                            <DropdownMenuItem @click="$emit('delete', comment)">
                                <TrashIcon class="mr-2 h-4 w-4" />
                                <span>Delete</span>
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            </div>

            <p class="text-sm">{{ comment.content }}</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem
} from '@/components/ui/dropdown-menu';
import { MoreVerticalIcon, PencilIcon, TrashIcon } from 'lucide-vue-next';
import { type PromptComment } from '@/utils/promptService';
import { useAuthStore } from '@/store/auth';

defineProps<{
    comments: PromptComment[];
}>();

defineEmits<{
    (e: 'edit', comment: PromptComment): void;
    (e: 'delete', comment: PromptComment): void;
}>();

const authStore = useAuthStore();
const currentUserId = computed(() => authStore.user?.id);
const isAdmin = computed(() => authStore.user?.roles?.includes('Admin') ?? false);

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
}

function canModify(comment: PromptComment): boolean {
    return comment.userId === currentUserId.value || isAdmin.value;
}
</script>