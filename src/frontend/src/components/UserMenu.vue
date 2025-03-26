<!-- src/Frontend/src/components/UserMenu.vue -->
<template>
    <DropdownMenu>
        <DropdownMenuTrigger as-child>
            <Button variant="ghost" class="relative h-8 flex items-center space-x-2">
                <Avatar class="h-8 w-8">
                    <AvatarImage :src="avatarUrl" />
                    <AvatarFallback>
                        {{ userInitials }}
                    </AvatarFallback>
                </Avatar>
                <span class="text-sm font-medium max-w-[100px] truncate">
                    {{ user?.userName }}
                </span>
            </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end" class="w-56">
            <DropdownMenuLabel>My Account</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="router.push('/profile')">
                Profile
            </DropdownMenuItem>
            <DropdownMenuItem @click="router.push('/settings')">
                Settings
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem @click="handleLogout">
                Logout
            </DropdownMenuItem>
        </DropdownMenuContent>
    </DropdownMenu>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from '@/components/ui/toast';
import { useAuth, type User } from '@/utils/authService';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator
} from '@/components/ui/dropdown-menu';
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';

const props = defineProps<{
    user: User | null
}>();

const router = useRouter();
const { logout } = useAuth();
const { toast } = useToast();

const userInitials = computed(() => {
    if (!props.user || !props.user.userName) return '?';
    return props.user.userName.charAt(0).toUpperCase();
});

const avatarUrl = computed(() => {
    // Could be replaced with a real avatar URL from user profile
    return '';
});

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