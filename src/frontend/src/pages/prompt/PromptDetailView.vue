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
                    <h1 class="text-3xl font-bold text-left">{{ prompt.title }}</h1>
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

            <!-- Models -->
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
                    <div v-if="prompt.latestVersion.images?.length" class="mt-6">
                        <h3 class="text-lg font-medium mb-3">Images</h3>
                        <ImageGallery :images="prompt.latestVersion.images" :can-delete="canEdit"
                            @delete="(imageId) => handleDeleteImage(imageId, prompt?.id)" />
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
            <Tabs v-model="currentTab" class="w-full">
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
                            <VersionDiff v-if="showVersionComparison && oldVersion && newVersion"
                                :old-version="oldVersion" :new-version="newVersion" @close="closeComparison"
                                class="mb-6" />

                            <VersionList :versions="versions" :active-version-id="activeVersionId" :can-edit="canEdit"
                                @select="selectVersion" @add-images="openAddImagesModal"
                                @delete-image="handleDeleteImage" @delete-version="handleDeleteVersion" @compare="handleCompareVersions"/>
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

        <Dialog :open="showAddImagesModal" @update:open="showAddImagesModal = $event">
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Add Images to Version</DialogTitle>
                    <DialogDescription>
                        Upload additional images for this prompt version.
                    </DialogDescription>
                </DialogHeader>

                <form @submit.prevent="handleAddImages">
                    <div class="space-y-4">
                        <ImageUploader v-model="addImagesForm.images"
                            v-model:captionsValue="addImagesForm.imageCaptions" />

                        <DialogFooter>
                            <Button type="button" variant="outline" @click="showAddImagesModal = false">
                                Cancel
                            </Button>
                            <Button type="submit" :disabled="isSubmittingImages || !addImagesForm.images.length">
                                <Spinner v-if="isSubmittingImages" class="mr-2 h-4 w-4" />
                                Upload Images
                            </Button>
                        </DialogFooter>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, reactive } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import { toast } from '@/components/ui/toast';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { PencilIcon, TrashIcon, PlusIcon } from 'lucide-vue-next';
import Spinner from '@/components/Spinner.vue';
import VersionForm from '@/components/VersionForm.vue';
import VersionList from '@/components/VersionList.vue';
import CommentForm from '@/components/CommentForm.vue';
import CommentList from '@/components/CommentList.vue';
import ImageGallery from '@/components/prompt/ImageGallery.vue';
import ImageUploader from '@/components/prompt/ImageUploader.vue';
import { usePromptStore } from '@/store/promptStore';
import { useAuthStore } from '@/store/auth';
import { type PromptComment } from '@/utils/promptService';
import VersionDiff from '@/components/prompt/VersionDiff.vue';

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
const currentTab = ref('versions');
const showVersionComparison = ref(false);
const comparisonVersions = ref<{ old: string, new: string } | null>(null);

const showAddImagesModal = ref(false);
const selectedVersionId = ref('');
const addImagesForm = reactive({
    images: [] as File[],
    imageCaptions: {} as Record<string, string>
});
const isSubmittingImages = ref(false);

const prompt = computed(() => promptStore.currentPrompt);
const versions = computed(() => promptStore.versions);
const comments = computed(() => promptStore.comments);
const isLoading = computed(() => promptStore.isLoading);
const isAuthenticated = computed(() => authStore.isAuthenticated);
const currentUserId = computed(() => authStore.user?.id);
const isAdmin = computed(() => authStore.user?.roles?.includes('Admin') ?? false);

const oldVersion = computed(() => {
    if (!comparisonVersions.value) return null;
    return versions.value.find(v => v.id === comparisonVersions.value?.old) || null;
});
const newVersion = computed(() => {
    if (!comparisonVersions.value) return null;
    return versions.value.find(v => v.id === comparisonVersions.value?.new) || null;
});


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

function handleCompareVersions(oldVersionId: string, newVersionId: string) {
    comparisonVersions.value = {
        old: oldVersionId,
        new: newVersionId
    };
    showVersionComparison.value = true;
}

function closeComparison() {
    showVersionComparison.value = false;
    comparisonVersions.value = null;
}

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

async function createVersion(version: { promptContent: string, images: File[], imageCaptions: Record<string, string> }) {
    if (!promptId.value) return;

    try {
        await promptStore.createVersion(promptId.value, version);
        showNewVersionForm.value = false;
        await loadPromptData();

        toast({
            title: 'Version created',
            description: 'New version has been created successfully.'
        });
    } catch (error) {
        console.error('Failed to create version:', error);
        toast({
            title: 'Error',
            description: 'Failed to create version. Please try again.',
            variant: 'destructive'
        });
    }
}

function openAddImagesModal(versionId: string) {
    selectedVersionId.value = versionId;
    addImagesForm.images = [];
    addImagesForm.imageCaptions = {};
    showAddImagesModal.value = true;
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

async function handleAddImages() {
    if (!selectedVersionId.value || addImagesForm.images.length === 0) return;

    isSubmittingImages.value = true;
    try {
        await promptStore.addImagesToVersion(
            selectedVersionId.value,
            addImagesForm.images,
            addImagesForm.imageCaptions
        );

        toast({
            title: 'Images added',
            description: 'Images have been added successfully.'
        });

        showAddImagesModal.value = false;
        await loadPromptData(); // Refresh data to show new images
    } catch (error) {
        console.error('Error adding images:', error);
        toast({
            title: 'Error',
            description: 'Failed to add images. Please try again.',
            variant: 'destructive'
        });
    } finally {
        isSubmittingImages.value = false;
    }
}

async function handleDeleteVersion(versionId: string, promptId?: string) {
    if (!promptId && prompt.value) {
        promptId = prompt.value.id;
    }

    if (!promptId) return;

    if (!confirm('Are you sure you want to delete this prompt version?')) return;

    try {
        await promptStore.deletePromptVersion(versionId, promptId);

        toast({
            title: 'Image deleted',
            description: 'Image has been deleted successfully.'
        });

        await loadPromptData(); // Refresh data to show updated images
    } catch (error) {
        console.error('Error deleting image:', error);
        toast({
            title: 'Error',
            description: 'Failed to delete image. Please try again.',
            variant: 'destructive'
        });
    }
}

async function handleDeleteImage(imageId: string, promptId?: string) {
    if (!promptId && prompt.value) {
        promptId = prompt.value.id;
    }

    if (!promptId) return;

    if (!confirm('Are you sure you want to delete this image?')) return;

    try {
        await promptStore.deleteVersionImage(imageId, promptId);

        toast({
            title: 'Image deleted',
            description: 'Image has been deleted successfully.'
        });

        await loadPromptData(); // Refresh data to show updated images
    } catch (error) {
        console.error('Error deleting image:', error);
        toast({
            title: 'Error',
            description: 'Failed to delete image. Please try again.',
            variant: 'destructive'
        });
    }
}

</script>