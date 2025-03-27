<template>
    <Card>
        <CardHeader>
            <CardTitle>Email Settings</CardTitle>
            <CardDescription>
                Manage your email verification status.
            </CardDescription>
        </CardHeader>
        <CardContent>
            <div class="space-y-4">
                <div class="flex items-center justify-between">
                    <div>
                        <h4 class="text-sm font-medium">Email Verification Status</h4>
                        <p class="text-sm text-muted-foreground">
                            {{ profile?.email }}
                        </p>
                    </div>
                    <Badge :variant="profile?.emailConfirmed ? 'default' : 'outline'">
                        {{ profile?.emailConfirmed ? "Verified" : "Not Verified" }}
                    </Badge>
                </div>

                <div v-if="!profile?.emailConfirmed" class="space-y-2">
                    <p class="text-sm text-muted-foreground">
                        Your email is not verified. Please verify your email to access all features.
                    </p>
                </div>
            </div>
        </CardContent>
    </Card>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useAccountStore } from "@/store/account";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

const accountStore = useAccountStore();

const profile = computed(() => accountStore.profile);
</script>