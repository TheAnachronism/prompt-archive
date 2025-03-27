<template>
    <DropdownMenu v-if="isAuthenticated">
        <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" class="relative h-8 w-8 rounded-full">
                {{ currentUser?.userName.charAt(0) || 'U' }}
            </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" class="w-56">
            <DropdownMenuLabel>My Account</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="router.push('/my-prompts')">
                <FileText class="mr-2 h-4 w-4" />
                <span>My Prompts</span>
            </DropdownMenuItem>
            <DropdownMenuItem @click="router.push('/settings')">
                <Settings class="mr-2 h-4 w-4" />
                <span>Settings</span>
            </DropdownMenuItem>
            <div v-if="isAdmin">
                <DropdownMenuSeparator />
                <DropdownMenuItem @click="router.push('/admin/users')">
                    <Settings class="mr-2 h-4 w-4" />
                    <span>Admin Settings</span>
                </DropdownMenuItem>
            </div>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="handleLogout">
                <LogOut class="mr-2 h-4 w-4" />
                <span>Log out</span>
            </DropdownMenuItem>
        </DropdownMenuContent>
    </DropdownMenu>
    <div v-else class="flex space-x-2">
        <Button variant="ghost" size="sm" @click="router.push('/login')">
            Login
        </Button>
        <Button variant="default" size="sm" @click="router.push('/register')">
            Sign Up
        </Button>
    </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useToast } from '@/components/ui/toast';
import { useAuth } from '@/utils/authService';
import { useAuthStore } from '@/store/auth';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { computed } from 'vue';
import { User, FileText, Settings, LogOut } from 'lucide-vue-next';

const router = useRouter();
const { currentUser, logout, isAuthenticated } = useAuth();
const authStore = useAuthStore();
const { toast } = useToast();

const isAdmin = computed(() => {
    return authStore.user?.roles?.includes('Admin') || false;
})

const handleLogout = async () => {
    try {
        await logout();
        toast({
            title: 'Logged out',
            description: 'You have been successfully logged out.',
        });
        router.push('/login');
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to log out. Please try again.',
            variant: 'destructive'
        });
    }
};
</script>