<!-- src/views/admin/AdminLayout.vue -->
<template>
    <div class="flex min-h-screen">
        <!-- Sidebar -->
        <div class="w-64 bg-card border-r border-border hidden md:block">
            <div class="p-4 border-b border-border">
                <h2 class="text-lg font-semibold">Admin Dashboard</h2>
            </div>
            <nav class="p-2">
                <ul class="space-y-1">
                    <li>
                        <router-link to="/admin/users"
                            class="flex items-center px-3 py-2 rounded-md text-sm transition-colors hover:bg-accent hover:text-accent-foreground"
                            :class="{ 'bg-accent text-accent-foreground': isActive('/admin/users') }">
                            <Users class="h-4 w-4 mr-2" />
                            User Management
                        </router-link>
                    </li>
                    <!-- Add more admin navigation items here -->
                </ul>
            </nav>
        </div>

        <div class="flex-1 flex flex-col">
            <header class="bg-card border-b border-border p-4 flex items-center justify-between">
                <div class="md:hidden">

                </div>
                <h1 class="text-xl font-semibold hidden md:block">Admin Dashboard</h1>
                <div class="flex items-center space-x-4">
                    <router-link to="/" class="text-sm text-muted-foreground hover:text-foreground">
                        Back to Site
                    </router-link>
                </div>
            </header>
            <main class="flex-1 p-6 overflow-auto">
                <router-view />
            </main>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';
import { Users } from 'lucide-vue-next';

const route = useRoute();

const isActive = (path: string) => {
    return route.path.startsWith(path);
};
</script>