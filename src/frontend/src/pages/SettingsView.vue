<template>
    <div class="container py-10 max-w-4xl mx-auto">
        <div class="space-y-6">
            <div>
                <h2 class="text-3xl font-bold tracking-tight">Account Settings</h2>
                <p class="text-muted-foreground">
                    Manage your account settings and preferences.
                </p>
            </div>

            <Tabs default-value="profile" class="w-full">
                <TabsList class="grid w-full grid-cols-3">
                    <TabsTrigger value="profile">Profile</TabsTrigger>
                    <TabsTrigger value="password">Password</TabsTrigger>
                    <TabsTrigger value="email">Email</TabsTrigger>
                </TabsList>
                <TabsContent value="profile">
                    <ProfileSettings />
                </TabsContent>
                <TabsContent value="password">
                    <PasswordSettings />
                </TabsContent>
                <TabsContent value="email">
                    <EmailSettings />
                </TabsContent>
            </Tabs>
        </div>
    </div>
</template>

<script  setup lang="ts">
import { onMounted } from "vue";
import { useAccountStore } from "@/store/account";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import ProfileSettings from "@/components/settings/ProfileSettings.vue";
import PasswordSettings from "@/components/settings/PasswordSettings.vue";
import EmailSettings from "@/components/settings/EmailSettings.vue";

const accountStore = useAccountStore();

onMounted(async () => {
    await accountStore.fetchProfile();
});
</script>