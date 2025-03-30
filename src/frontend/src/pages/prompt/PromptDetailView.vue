<template>
    <div class="space-y-8">
        <div v-if="isLoading" class="flex justify-center py-12">
            <Spinner class="h-8 w-8" />
        </div>

        <div v-else-if="!prompt" class="text-center py-12">
            <div class="text-muted-foreground">Prompt not found</div>
            <Button class="mt-4" @click="router.push('/prompts')">
                Back to Prompts
            </Button>
        </div>

        <template v-else>
            <!-- Header -->
            <div class="flex flex-col md:flex-row justify-between items-start gap-4">
                <div>
                    <div class="flex items-center gap-2 text-sm text-muted-foreground mb-2">
                        <Button variant="ghost" size="sm" class="p-0 h-auto" @click="router.push('/prompts')">
                            Prompts
                        </Button>
                        <span>/</span>
                        <span>{{ prompt.title }}</span>
                    </div>
                    <h1 class="text-3xl font-bold">{{ prompt.title }}</h1>
                    <div class="text-muted-foreground mt-1">
                        By {{ prompt.userName }} · Created {{ formatDate(prompt.createdAt) }}
                    </div>
                </div>

                <div class="flex gap-2">
                    <Button v-if="canEdit" variant="outline" @click="router.push(`/prompts/${prompt.id}/edit`)">
                        <PencilIcon class="h-4 w-4 mr-2" />
                        Edit
                    </Button>

                    <Button v-if="canEdit" variant="outline" @click="showNewVersionForm = true">
                        <PlusIcon class="h-4 w-4 mr-2" />
                        New Version
                    </Button>

                    <Button v-if="canDelete" variant="destructive" @click="confirmDelete">
                        <TrashIcon class="h-4 w-4 mr-2" />
                        Delete
                    </Button>
                </div>
            </div>

            <!-- Models -->>
            <div class="flex flex-wrap gap-2">
                <Badge v-for="model in prompt.models" :key="model" variant="default">
                    {{ model }}
                </Badge>
            </div>

            <!-- Tags -->
            <div class="flex flex-wrap gap-2">
                <Badge v-for="tag in prompt.tags" :key="tag" variant="secondary">
                    {{ tag }}
                </Badge>
            </div>

            <!-- Description -->
            <Card>
                <CardHeader>
                    <CardTitle>Description</CardTitle>
                </CardHeader>
                <CardContent>
                    <p class="whitespace-pre-wrap">{{ prompt.description || 'No description provided.' }}</p>
                </CardContent>
            </Card>

            <!-- Latest Version Content -->
            <Card v-if="prompt.latestVersion">
                <CardHeader>
                    <CardTitle class="flex justify-between items-center">
                        <span>Latest Prompt (Version {{ prompt.latestVersion.versionNumber }})</span>
                        <Badge variant="outline">
                            {{ formatDate(prompt.latestVersion.createdAt) }}
                        </Badge>
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div class="bg-muted p-4 rounded-lg whitespace-pre-wrap">
                        {{ prompt.latestVersion.promptContent }}
                    </div>
                </CardContent>
            </Card>

            <!-- New Version Form -->
            <Card v-if="showNewVersionForm">
                <CardHeader>
                    <CardTitle>Create New Version</CardTitle>
                </CardHeader>
                <CardContent>
                    <VersionForm :initial-content="prompt.latestVersion?.promptContent" :is-loading="isLoading"
                        @submit="createVersion" @cancel="showNewVersionForm = false" />
                </CardContent>
            </Card>

            <!-- Tabs for Versions and Comments -->
            <Tabs default-value="versions" class="w-full">
                <TabsList class="grid w-full grid-cols-2">
                    <TabsTrigger value="versions">
                        Versions ({{ prompt.versionCount }})
                    </TabsTrigger>
                    <TabsTrigger value="comments">
                        Comments ({{ prompt.commentCount }})
                    </TabsTrigger>
                </TabsList>

                <TabsContent value="versions">
                    <Card>
                        <CardContent class="pt-6">
                            <VersionList :versions="versions" :active-version-id="activeVersionId"
                                @select="selectVersion" />
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value="comments">
                    <div class="space-y-6">
                        <Card v-if="isAuthenticated && !editingComment">
                            <CardContent class="pt-6">
                                <CommentForm :is-loading="isLoading" @submit="addComment" />
                            </CardContent>
                        </Card>

                        <Card v-if="editingComment">
                            <CardHeader>
                                <CardTitle>Edit Comment</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <CommentForm :initial-content="editingComment.content" :is-loading="isLoading"
                                    :is-edit-mode="true" @submit="updateComment" @cancel="editingComment = null" />
                            </CardContent>
                        </Card>

                        <Card>
                            <CardContent class="pt-6">
                                <CommentList :comments="comments" @edit="startEditingComment"
                                    @delete="confirmDeleteComment" />
                            </CardContent>
                        </Card>
                    </div>
                </TabsContent>
            </Tabs>
        </template>

        <!-- Delete Confirmation Dialog -->
        <Dialog :open="showDeleteDialog" @update:open="showDeleteDialog = $event">
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Delete Prompt</DialogTitle>
                    <DialogDescription>
                        Are you sure you want to delete this prompt? This action cannot be undone.
                    </DialogDescription>
                </DialogHeader>
                <DialogFooter>
                    <Button variant="outline" @click="showDeleteDialog = false">
                        Cancel
                    </Button>
                    <Button variant="destructive" @click="deletePrompt">
                        Delete
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>

        <!-- Delete Comment Confirmation Dialog -->
        <Dialog :open="showDeleteCommentDialog" @update:open="showDeleteCommentDialog = $event">
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Delete Comment</DialogTitle>
                    <DialogDescription>
                        Are you sure you want to delete this comment? This action cannot be undone.
                    </DialogDescription>
                </DialogHeader>
                <DialogFooter>
                    <Button variant="outline" @click="showDeleteCommentDialog = false">
                        Cancel
                    </Button>
                    <Button variant="destructive" @click="deleteComment">
                        Delete
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { PencilIcon, TrashIcon, PlusIcon } from 'lucide-vue-next';
import Spinner from '@/components/Spinner.vue';
import VersionForm from '@/components/VersionForm.vue';
import VersionList from '@/components/VersionList.vue';
import CommentForm from '@/components/CommentForm.vue';
import CommentList from '@/components/CommentList.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';
import { type PromptComment } from '@/utils/promptService';

const router = useRouter();
const route = useRoute();
const promptStore = usePromptStore();
const authStore = useAuthStore();

const promptId = computed(() => route.params.id as string);
const showNewVersionForm = ref(false);
const activeVersionId = ref<string | undefined>(undefined);
const editingComment = ref<PromptComment | null>(null);
const showDeleteDialog = ref(false);
const showDeleteCommentDialog = ref(false);
const commentToDelete = ref<PromptComment | null>(null);

const prompt = computed(() => promptStore.currentPrompt);
const versions = computed(() => promptStore.versions);
const comments = computed(() => promptStore.comments);
const isLoading = computed(() => promptStore.isLoading);
const isAuthenticated = computed(() => authStore.isAuthenticated);
const currentUserId = computed(() => authStore.user?.id);
const isAdmin = computed(() => authStore.user?.roles?.includes('Admin') ?? false);

const canEdit = computed(() => {
    if (!prompt.value || !isAuthenticated.value) return false;
    return prompt.value.userId === currentUserId.value || isAdmin.value;
});

const canDelete = computed(() => {
    if (!prompt.value || !isAuthenticated.value) return false;
    return prompt.value.userId === currentUserId.value || isAdmin.value;
});

onMounted(async () => {
    await loadPromptData();
});

watch(promptId, async () => {
    await loadPromptData();
});

async function loadPromptData() {
    if (!promptId.value) return;

    await promptStore.fetchPromptById(promptId.value);

    if (prompt.value) {
        await Promise.all([
            promptStore.fetchVersions(promptId.value),
            promptStore.fetchComments(promptId.value)
        ]);

        // Set the latest version as active by default
        if (prompt.value.latestVersion) {
            activeVersionId.value = prompt.value.latestVersion.id;
        }
    }
}

function formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString();
}

function selectVersion(versionId: string) {
    if (activeVersionId.value === versionId) {
        activeVersionId.value = undefined;
    } else {
        activeVersionId.value = versionId;
    }
}

async function createVersion(content: string) {
    if (!promptId.value) return;

    try {
        await promptStore.createVersion(promptId.value, { content });
        showNewVersionForm.value = false;
        await loadPromptData();
    } catch (error) {
        console.error('Failed to create version:', error);
    }
}

function confirmDelete() {
    showDeleteDialog.value = true;
}

async function deletePrompt() {
    if (!promptId.value) return;

    try {
        await promptStore.deletePrompt(promptId.value);
        showDeleteDialog.value = false;
        router.push('/prompts');
    } catch (error) {
        console.error('Failed to delete prompt:', error);
    }
}

async function addComment(content: string) {
    if (!promptId.value) return;

    try {
        await promptStore.addComment(promptId.value, { content });
    } catch (error) {
        console.error('Failed to add comment:', error);
    }
}

function startEditingComment(comment: PromptComment) {
    editingComment.value = comment;
}

async function updateComment(content: string) {
    if (!editingComment.value) return;

    try {
        await promptStore.updateComment(editingComment.value.id, { content });
        editingComment.value = null;
    } catch (error) {
        console.error('Failed to update comment:', error);
    }
}

function confirmDeleteComment(comment: PromptComment) {
    commentToDelete.value = comment;
    showDeleteCommentDialog.value = true;
}

async function deleteComment() {
    if (!commentToDelete.value || !promptId.value) return;

    try {
        await promptStore.deleteComment(commentToDelete.value.id, promptId.value);
        showDeleteCommentDialog.value = false;
        commentToDelete.value = null;
    } catch (error) {
        console.error('Failed to delete comment:', error);
    }
}
</script>