import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '@/utils/authService';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/prompts',
      component: () => import('@/layouts/MainLayout.vue'),
      children: [
        {
          path: 'login',
          name: 'login',
          component: () => import('@/pages/LoginView.vue'),
          meta: { guestOnly: true }
        },
        {
          path: 'register',
          name: 'register',
          component: () => import('@/pages/RegisterView.vue'),
          meta: { guestOnly: true }
        },
        {
          path: '/admin',
          name: 'admin',
          component: () => import('@/pages/admin/AdminLayout.vue'),
          meta: { requiresAuth: true, requiresAdmin: true },
          children: [
            {
              path: 'users',
              name: 'admin-users',
              component: () => import('@/pages/admin/UsersView.vue'),
              meta: { requiresAuth: true, requiresAdmin: true }
            },
            // Add more admin routes as needed
          ]
        },
        {
          path: '/settings',
          name: 'Settings',
          component: () => import('@/pages/SettingsView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: '/prompts',
          component: () => import('@/pages/prompt/PromptListView.vue')
        },
        {
          path: '/prompts/create',
          component: () => import('@/pages/prompt/CreatePromptView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: '/prompts/:id',
          component: () => import('@/pages/prompt/PromptDetailView.vue')
        },
        {
          path: '/prompts/:id/edit',
          component: () => import('@/pages/prompt/EditPromptView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: '/my-prompts',
          component: () => import('@/pages/prompt/MyPromptsView.vue'),
          meta: { requiresAuth: true }
        },
        // {
        //   path: 'forgot-password',
        //   name: 'forgotPassword',
        //   component: ForgotPasswordPage,
        //   meta: { guestOnly: true }
        // },
        // {
        //   path: 'dashboard',
        //   name: 'dashboard',
        //   component: DashboardView,
        //   meta: { requiresAuth: true }
        // }
      ]
    }
  ]
})

router.beforeEach(async (to, _from, next) => {
  const { isAuthenticated, isLoading, checkAuthStatus } = useAuth();

  if (isLoading.value)
    await checkAuthStatus();

  if (to.meta.requiresAuth && !isAuthenticated.value)
    return next({ name: 'login', query: { redirect: to.fullPath } });

  if (to.meta.guestOnly && isAuthenticated.value)
    return next({ name: 'home' });

  next();
});

router.afterEach((to, from) => {
  console.log(`Navigation: ${from.path} -> ${to.path}`);
})

export default router
