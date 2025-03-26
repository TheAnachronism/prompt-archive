<template>
    <div class="min-h-screen flex flex-col">
        <header
            class="border-grid sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
            <div class="container-wrapper">
                <div class="container flex h-14 items-center">
                    <div class="mr-4 md:mr-1 hidden md:flex">
                        <a href="/" class="mr-4 md:mr-2 lg:mr-6 flex items-center lg:space-x-1 xl:space-x-2 flex-1">
                            <div class="h-6 w-6">
                                <img src="@/assets/vue.svg" class="h-6 w-6" />
                            </div>
                            <span class="font-bold"> Prompt Archive </span>
                        </a>
                    </div>
                    <div class="flex flex-1 items-center justify-between space-x-2 md:justify-end"></div>
                </div>
            </div>
        </header>

        <!-- Main Content Area -->
        <main class="flex flex-1 flex-col">
            <router-view />
        </main>

        <ToastProvider />
    </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import { useAuth } from '@/utils/authService';
import {
    NavigationMenu,
    NavigationMenuContent,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    NavigationMenuTrigger,
} from '@/components/ui/navigation-menu';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
    Sheet,
    SheetContent,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
} from '@/components/ui/sheet';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import ToastProvider from '@/components/ToastProvider.vue';
import { Menu, User, FileText, Settings, LogOut } from 'lucide-vue-next';

const router = useRouter();
const { currentUser, isAuthenticated } = useAuth();
</script>