<template>
    <div class="min-h-screen flex flex-col">
        <header class="border-b border-border bg-background w-full">
            <div class="container mx-auto px-4 py-2 flex items-center justify-between">
                <!-- Left section: Logo & Navigation -->
                <div class="flex items-center space-x-8">
                    <router-link to="/" class="flex items-center">
                        <h1 class="font-bold text-xl text-primary">Prompt Archive</h1>
                    </router-link>

                    <!-- Main Navigation -->
                    <nav class="hidden md:flex items-center space-x-6">
                        <NavigationMenu>
                            <NavigationMenuList>
                                <NavigationMenuItem>
                                    <NavigationMenuTrigger>Explore</NavigationMenuTrigger>
                                    <NavigationMenuContent>
                                        <ul class="grid gap-3 p-4 w-[400px]">
                                            <li>
                                                <NavigationMenuLink asChild>
                                                    <router-link to="/popular"
                                                        class="block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground">
                                                        <div class="text-sm font-medium leading-none">Popular</div>
                                                        <p
                                                            class="line-clamp-2 text-sm leading-snug text-muted-foreground">
                                                            Discover trending AI image prompts
                                                        </p>
                                                    </router-link>
                                                </NavigationMenuLink>
                                            </li>
                                            <li>
                                                <NavigationMenuLink asChild>
                                                    <router-link to="/recent"
                                                        class="block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground">
                                                        <div class="text-sm font-medium leading-none">Recent</div>
                                                        <p
                                                            class="line-clamp-2 text-sm leading-snug text-muted-foreground">
                                                            Browse the latest additions to our collection
                                                        </p>
                                                    </router-link>
                                                </NavigationMenuLink>
                                            </li>
                                        </ul>
                                    </NavigationMenuContent>
                                </NavigationMenuItem>
                                <NavigationMenuItem>
                                    <NavigationMenuLink asChild>
                                        <router-link to="/create"
                                            class="group inline-flex h-10 w-max items-center justify-center rounded-md bg-background px-4 py-2 text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground focus:outline-none disabled:pointer-events-none disabled:opacity-50">
                                            Create
                                        </router-link>
                                    </NavigationMenuLink>
                                </NavigationMenuItem>
                            </NavigationMenuList>
                        </NavigationMenu>
                    </nav>
                </div>

                <!-- Right section: Search & User Profile -->
                <div class="flex items-center space-x-4">
                    <!-- Search -->
                    <div class="hidden md:block relative">
                        <Input type="search" placeholder="Search prompts..." class="w-[200px] md:w-[300px]" />
                    </div>

                    <!-- User Profile / Auth -->
                    <DropdownMenu v-if="isAuthenticated">
                        <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="icon" class="relative h-8 w-8 rounded-full">
                                {{  currentUser?.initials || 'U' }}
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end" class="w-56">
                            <DropdownMenuLabel>My Account</DropdownMenuLabel>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem @click="router.push('/profile')">
                                <User class="mr-2 h-4 w-4" />
                                <span>Profile</span>
                            </DropdownMenuItem>
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
                            <DropdownMenuItem @click="logout">
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
                </div>
            </div>
        </header>

        <!-- Mobile Navigation -->
        <Sheet>
            <SheetTrigger asChild class="md:hidden absolute top-3 left-4">
                <Button variant="ghost" size="icon">
                    <Menu class="h-5 w-5" />
                </Button>
            </SheetTrigger>
            <SheetContent side="left" class="w-[250px] sm:w-[300px]">
                <SheetHeader>
                    <SheetTitle>Prompt Archive</SheetTitle>
                </SheetHeader>
                <nav class="flex flex-col space-y-4 mt-6">
                    <router-link to="/" class="px-2 py-1 rounded-md hover:bg-accent hover:text-accent-foreground">
                        Home
                    </router-link>
                    <router-link to="/popular"
                        class="px-2 py-1 rounded-md hover:bg-accent hover:text-accent-foreground">
                        Popular
                    </router-link>
                    <router-link to="/recent" class="px-2 py-1 rounded-md hover:bg-accent hover:text-accent-foreground">
                        Recent
                    </router-link>
                    <router-link to="/create" class="px-2 py-1 rounded-md hover:bg-accent hover:text-accent-foreground">
                        Create
                    </router-link>
                </nav>
            </SheetContent>
        </Sheet>

        <!-- Main Content Area -->
        <main class="flex-grow container mx-auto px-4 py-6">
            <router-view />
        </main>

        <!-- Footer -->
        <footer class="border-t border-border py-4 text-center text-sm text-muted-foreground">
            <div class="container mx-auto">
                &copy; {{ new Date().getFullYear() }} Prompt Archive
            </div>
        </footer>

        <!-- Toast Container -->
        <Toaster />
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
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Toaster } from '@/components/ui/toast';
import { Menu, User, FileText, Settings, LogOut } from 'lucide-vue-next';
import { useAuthStore } from '@/store/auth';
import { computed } from 'vue';

const router = useRouter();
const { currentUser, isAuthenticated, logout } = useAuth();
const authStore = useAuthStore();

const isAdmin = computed(() => {
    return authStore.user?.roles?.includes('Admin') || false;
})
</script>