<template>
    <form @submit.prevent="handleSubmit" class="space-y-6">
        <div class="space-y-2">
            <Label for="content">New Version Content</Label>
            <Textarea id="content" v-model="content" rows="8" placeholder="Enter your updated prompt text here..."
                required />
        </div>

        <div class="flex justify-end gap-2">
            <Button type="button" variant="outline" @click="$emit('cancel')">
                Cancel
            </Button>
            <Button type="submit" :disabled="isLoading">
                Create Version
            </Button>
        </div>
    </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';

const props = defineProps<{
    initialContent?: string;
    isLoading?: boolean;
}>();

const emit = defineEmits<{
    (e: 'submit', content: string): void;
    (e: 'cancel'): void;
}>();

const content = ref(props.initialContent || '');

function handleSubmit() {
    emit('submit', content.value);
}
</script>