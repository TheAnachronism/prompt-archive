<template>
    <Combobox v-model="selectedItem">
        <div class="flex gap-2 w-full">
            <ComboboxAnchor class="flex-grow">
                <ComboboxInput id="item-input" v-model="input" :placeholder="props.placeholder" class="w-full"
                    @keydown.enter.prevent="addItem" />
            </ComboboxAnchor>
            <Button v-if="!props.hideAddButton" type="button" variant="outline" size="sm" @click="addItem">
                <PlusIcon class="h-4 w-4 mr-1" />
                Add
            </Button>
        </div>

        <!-- Full-width dropdown -->
        <ComboboxList class="w-[calc(100%+56px)] max-h-60 overflow-auto">
            <ComboboxEmpty>No results found</ComboboxEmpty>

            <ComboboxItem v-for="item in filteredItems" :key="item" :value="item" class="py-2">
                {{ item }}
                <ComboboxItemIndicator>
                    <Check class="ml-auto h-4 w-4" />
                </ComboboxItemIndicator>
            </ComboboxItem>
        </ComboboxList>
    </Combobox>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { Button } from '@/components/ui/button';
import {
    Combobox,
    ComboboxAnchor,
    ComboboxInput,
    ComboboxList,
    ComboboxEmpty,
    ComboboxItem,
    ComboboxItemIndicator
} from '@/components/ui/combobox';
import { PlusIcon, Check } from 'lucide-vue-next';

const selectedItem = ref('');
const input = ref('');

const props = defineProps<{
    items: string[];
    availableItems: string[];
    placeholder: string;
    setError?: (error: string) => void;
    addUnknownItems?: boolean;
    itemsAddedHandler?: () => void;
    hideAddButton?: boolean
}>();

const filteredItems = computed(() => {
    const lowerInput = input.value.toLowerCase();
    return props.availableItems
        .filter(item => item.toLowerCase().includes(lowerInput))
        .filter(item => !props.items.includes(item));
});

watch(selectedItem, (newItem) => {
    if (newItem && !props.items.includes(newItem)) {
        props.items.push(newItem);
        triggerHandled();
        input.value = '';
        selectedItem.value = '';
    }
});

function addItem() {
    if (selectedItem.value.length > 1)
        props.items.push(selectedItem.value);
    else if (input.value.trim() && !props.items.includes(input.value.trim()))
        props.items.push(input.value.trim());

    triggerHandled();
    if (props.setError)
        props.setError('');

    input.value = '';
    selectedItem.value = '';
}

function triggerHandled() {
    if (props.itemsAddedHandler)
        props.itemsAddedHandler();
}

</script>