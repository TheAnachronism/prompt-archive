<template>
    <div class="container mx-auto py-6">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-2xl font-bold">User Management</h1>
            <Button @click="openCreateDialog">
                <PlusIcon class="h-4 w-4 mr-2" />
                Add User
            </Button>
        </div>

        <div class="bg-card rounded-lg shadow overflow-hidden">
            <div class="p-4 border-b border-border flex justify-between items-center">
                <div class="relative w-64">
                    <Search class="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                    <Input v-model="searchTerm" placeholder="Search users..." class="pl-8" @input="debouncedSearch" />
                </div>
                <div class="flex items-center space-x-2">
                    <Select v-model="pageSize" @update:modelValue="loadUsers">
                        <option value="5">5 per page</option>
                        <option value="10">10 per page</option>
                        <option value="25">25 per page</option>
                        <option value="50">50 per page</option>
                    </Select>
                </div>
            </div>

            <div class="overflow-x-auto">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Username</TableHead>
                            <TableHead>Email</TableHead>
                            <TableHead>Roles</TableHead>
                            <TableHead>Email Confirmed</TableHead>
                            <TableHead>Created</TableHead>
                            <TableHead>Last Login</TableHead>
                            <TableHead class="text-right">Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        <TableRow v-for="user in users" :key="user.id">
                            <TableCell>{{ user.userName }}</TableCell>
                            <TableCell>{{ user.email }}</TableCell>
                            <TableCell>
                                <div class="flex flex-wrap gap-1">
                                    <Badge v-for="role in user.roles" :key="role"
                                        :variant="role === 'Admin' ? 'destructive' : 'secondary'">
                                        {{ role }}
                                    </Badge>
                                </div>
                            </TableCell>
                            <TableCell>
                                <CheckIcon v-if="user.emailConfirmed" class="h-5 w-5 text-green-500" />
                                <XIcon v-else class="h-5 w-5 text-red-500" />
                            </TableCell>
                            <TableCell>{{ formatDate(user.createdAt) }}</TableCell>
                            <TableCell>{{ user.lastLoginAt ? formatDate(user.lastLoginAt) : 'Never' }}</TableCell>
                            <TableCell class="text-right">
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant="ghost" size="icon">
                                            <MoreHorizontal class="h-4 w-4" />
                                        </Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent align="end">
                                        <DropdownMenuItem @click="editUser(user)">
                                            <Edit class="h-4 w-4 mr-2" />
                                            Edit
                                        </DropdownMenuItem>
                                        <DropdownMenuItem @click="openChangePasswordDialog(user)">
                                            <Key class="h-4 w-4 mr-2" />
                                            Change Password
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />
                                        <DropdownMenuItem @click="confirmDelete(user)"
                                            class="text-destructive focus:text-destructive">
                                            <Trash class="h-4 w-4 mr-2" />
                                            Delete
                                        </DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                            </TableCell>
                        </TableRow>
                        <TableRow v-if="users.length === 0">
                            <TableCell colspan="7" class="text-center py-8 text-muted-foreground">
                                No users found
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
            </div>

            <div class="p-4 border-t border-border flex justify-between items-center">
                <div class="text-sm text-muted-foreground">
                    Showing {{ users.length }} of {{ totalUsers }} users
                </div>
                <div class="flex items-center space-x-2">
                    <Button variant="outline" size="sm" :disabled="currentPage === 1" @click="prevPage">
                        Previous
                    </Button>
                    <span class="text-sm">
                        Page {{ currentPage }} of {{ totalPages }}
                    </span>
                    <Button variant="outline" size="sm" :disabled="currentPage === totalPages" @click="nextPage">
                        Next
                    </Button>
                </div>
            </div>
        </div>

        <!-- Create/Edit User Dialog -->
        <Dialog :open="isDialogOpen" @update:open="isDialogOpen = $event">
            <DialogContent class="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>{{ isEditing ? 'Edit User' : 'Create User' }}</DialogTitle>
                    <DialogDescription>
                        {{ isEditing ? 'Update user details' : 'Add a new user to the system' }}
                    </DialogDescription>
                </DialogHeader>
                <form @submit.prevent="isEditing ? updateUser() : createUser()">
                    <div class="grid gap-4 py-4">
                        <div class="grid gap-2">
                            <Label for="username">Username</Label>
                            <Input id="username" v-model="userForm.userName" placeholder="Enter username" required />
                        </div>
                        <div class="grid gap-2">
                            <Label for="email">Email</Label>
                            <Input id="email" v-model="userForm.email" type="email" placeholder="Enter email"
                                required />
                        </div>
                        <div v-if="!isEditing" class="grid gap-2">
                            <Label for="password">Password</Label>
                            <Input id="password" v-model="userForm.password" type="password"
                                placeholder="Enter password" required />
                        </div>
                        <div class="grid gap-2">
                            <Label for="roles">Roles</Label>
                            <div class="flex flex-wrap gap-2">
                                <div v-for="role in availableRoles" :key="role" class="flex items-center space-x-2">
                                    <Checkbox :id="`role-${role}`" :checked="userForm.roles.includes(role)"
                                        @update:checked="toggleRole(role)" />
                                    <Label :for="`role-${role}`" class="cursor-pointer">{{ role }}</Label>
                                </div>
                            </div>
                        </div>
                        <div v-if="isEditing" class="flex items-center space-x-2">
                            <Checkbox id="emailConfirmed" v-model:checked="userForm.emailConfirmed" />
                            <Label for="emailConfirmed">Email Confirmed</Label>
                        </div>
                    </div>
                    <DialogFooter>
                        <Button type="button" variant="outline" @click="isDialogOpen = false">
                            Cancel
                        </Button>
                        <Button type="submit">
                            {{ isEditing ? 'Update' : 'Create' }}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>

        <!-- Change Password Dialog -->
        <Dialog :open="isPasswordDialogOpen" @update:open="isPasswordDialogOpen = $event">
            <DialogContent class="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Change Password</DialogTitle>
                    <DialogDescription>
                        Set a new password for {{ selectedUser?.userName }}
                    </DialogDescription>
                </DialogHeader>
                <form @submit.prevent="changePassword">
                    <div class="grid gap-4 py-4">
                        <div class="grid gap-2">
                            <Label for="newPassword">New Password</Label>
                            <Input id="newPassword" v-model="passwordForm.newPassword" type="password"
                                placeholder="Enter new password" required />
                        </div>
                        <div class="grid gap-2">
                            <Label for="confirmPassword">Confirm Password</Label>
                            <Input id="confirmPassword" v-model="passwordForm.confirmPassword" type="password"
                                placeholder="Confirm new password" required />
                            <p v-if="passwordMismatch" class="text-sm text-destructive">
                                Passwords do not match
                            </p>
                        </div>
                    </div>
                    <DialogFooter>
                        <Button type="button" variant="outline" @click="isPasswordDialogOpen = false">
                            Cancel
                        </Button>
                        <Button type="submit" :disabled="passwordMismatch">
                            Change Password
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>

        <!-- Delete Confirmation Dialog -->
        <AlertDialog :open="isDeleteDialogOpen" @update:open="isDeleteDialogOpen = $event">
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                    <AlertDialogDescription>
                        This will permanently delete the user "{{ selectedUser?.userName }}".
                        This action cannot be undone.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel @click="isDeleteDialogOpen = false">Cancel</AlertDialogCancel>
                    <AlertDialogAction @click="deleteUser">Delete</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useToast } from '@/components/ui/toast';
import { format } from 'date-fns';
import debounce from 'lodash.debounce';
import {
    userService,
    type User,
    type CreateUserRequest,
    type UpdateUserRequest
} from '@/utils/userService';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow
} from '@/components/ui/table';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle
} from '@/components/ui/dialog';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle
} from '@/components/ui/alert-dialog';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { Badge } from '@/components/ui/badge';
import { Select } from '@/components/ui/select';
import {
    PlusIcon,
    Search,
    MoreHorizontal,
    Edit,
    Trash,
    Key,
    CheckIcon,
    XIcon
} from 'lucide-vue-next';

const { toast } = useToast();

// User list state
const users = ref<User[]>([]);
const totalUsers = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const searchTerm = ref('');
const isLoading = ref(false);

// Dialog state
const isDialogOpen = ref(false);
const isPasswordDialogOpen = ref(false);
const isDeleteDialogOpen = ref(false);
const isEditing = ref(false);
const selectedUser = ref<User | null>(null);
const availableRoles = ref<string[]>([]);

// Form state
const userForm = ref<CreateUserRequest & UpdateUserRequest>({
    userName: '',
    email: '',
    password: '',
    roles: [],
    emailConfirmed: false
});

const passwordForm = ref({
    newPassword: '',
    confirmPassword: ''
});

const passwordMismatch = computed(() => {
    return (
        passwordForm.value.newPassword !== '' &&
        passwordForm.value.confirmPassword !== '' &&
        passwordForm.value.newPassword !== passwordForm.value.confirmPassword
    );
});

const totalPages = computed(() => {
    return Math.ceil(totalUsers.value / pageSize.value);
});

// Load users with pagination and search
const loadUsers = async () => {
    isLoading.value = true;
    try {
        const response = await userService.getUsers(
            currentPage.value,
            pageSize.value,
            searchTerm.value
        );
        users.value = response.users;
        totalUsers.value = response.totalCount;
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to load users',
            variant: 'destructive'
        });
    } finally {
        isLoading.value = false;
    }
};

// Load available roles
const loadRoles = async () => {
    try {
        availableRoles.value = await userService.getRoles();
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to load roles',
            variant: 'destructive'
        });
    }
};

// Pagination methods
const nextPage = () => {
    if (currentPage.value < totalPages.value) {
        currentPage.value++;
        loadUsers();
    }
};

const prevPage = () => {
    if (currentPage.value > 1) {
        currentPage.value--;
        loadUsers();
    }
};

// Search with debounce
const debouncedSearch = debounce(() => {
    currentPage.value = 1;
    loadUsers();
}, 300);

// Format date
const formatDate = (dateString: string) => {
    return format(new Date(dateString), 'MMM d, yyyy');
};

// Dialog methods
const openCreateDialog = () => {
    isEditing.value = false;
    userForm.value = {
        userName: '',
        email: '',
        password: '',
        roles: ['User'],
        emailConfirmed: true
    };
    isDialogOpen.value = true;
};

const editUser = (user: User) => {
    isEditing.value = true;
    selectedUser.value = user;
    userForm.value = {
        userName: user.userName,
        email: user.email,
        password: '',
        roles: [...user.roles],
        emailConfirmed: user.emailConfirmed
    };
    isDialogOpen.value = true;
};

const openChangePasswordDialog = (user: User) => {
    selectedUser.value = user;
    passwordForm.value = {
        newPassword: '',
        confirmPassword: ''
    };
    isPasswordDialogOpen.value = true;
};

const confirmDelete = (user: User) => {
    selectedUser.value = user;
    isDeleteDialogOpen.value = true;
};

// Toggle role selection
const toggleRole = (role: string) => {
    if (userForm.value.roles.includes(role)) {
        userForm.value.roles = userForm.value.roles.filter(r => r !== role);
    } else {
        userForm.value.roles.push(role);
    }
};

// API methods
const createUser = async () => {
    try {
        await userService.createUser(userForm.value);
        toast({
            title: 'Success',
            description: 'User created successfully',
            variant: 'default'
        });
        isDialogOpen.value = false;
        loadUsers();
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to create user',
            variant: 'destructive'
        });
    }
};

const updateUser = async () => {
    if (!selectedUser.value) return;

    try {
        await userService.updateUser(selectedUser.value.id, {
            userName: userForm.value.userName,
            email: userForm.value.email,
            roles: userForm.value.roles,
            emailConfirmed: userForm.value.emailConfirmed
        });
        toast({
            title: 'Success',
            description: 'User updated successfully',
            variant: 'default'
        });
        isDialogOpen.value = false;
        loadUsers();
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to update user',
            variant: 'destructive'
        });
    }
};

const changePassword = async () => {
    if (!selectedUser.value) return;
    if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
        return;
    }

    try {
        await userService.changePassword({
            userId: selectedUser.value.id,
            newPassword: passwordForm.value.newPassword
        });
        toast({
            title: 'Success',
            description: 'Password changed successfully',
            variant: 'default'
        });
        isPasswordDialogOpen.value = false;
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to change password',
            variant: 'destructive'
        });
    }
};

const deleteUser = async () => {
    if (!selectedUser.value) return;

    try {
        await userService.deleteUser(selectedUser.value.id);
        toast({
            title: 'Success',
            description: 'User deleted successfully',
            variant: 'default'
        });
        isDeleteDialogOpen.value = false;
        loadUsers();
    } catch (error) {
        toast({
            title: 'Error',
            description: 'Failed to delete user',
            variant: 'destructive'
        });
    }
};

// Initialize
onMounted(() => {
    loadUsers();
    loadRoles();
});
</script>