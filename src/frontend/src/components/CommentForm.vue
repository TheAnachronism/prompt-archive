<template>
    <form @submit.prevent="handleSubmit" class="space-y-4">
        <div class="space-y-2">
            <Textarea v-model="content" :placeholder="isEditMode ? 'Edit your comment...' : 'Add a comment...'" rows="3"
                required />
        </div>

        <div class="flex justify-end gap-2">
            <Button v-if="isEditMode" type="button" variant="outline" @click="$emit('cancel')">
                Cancel
            </Button>
            <Button type="submit" :disabled="isLoading">
                {{ isEditMode ? 'Update' : 'Comment' }}
            </Button>
        </div>
    </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';

const props = defineProps<{
    initialContent?: string;
    isLoading?: boolean;
    isEditMode?: boolean;
}>();

const emit = defineEmits<{
    (e: 'submit', content: string): void;
    (e: 'cancel'): void;
}>();

const content = ref(props.initialContent || '');

function handleSubmit() {
    emit('submit', content.value);
    if (!props.isEditMode) {
        content.value = '';
    }
}
</script>