<template>
    
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../utils/api'

interface HealthStatus {
    status: string
    timestamp: string
}

const health = ref<HealthStatus | null>(null)

onMounted(async () => {
    try {
        const response = await api.get('/health')
        health.value = response.data
    } catch (error) {
        console.error('Failed to fetch health status', error)
    }
})
</script>