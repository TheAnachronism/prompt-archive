import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '@/utils/authService';
import MainLayout from '@/layouts/MainLayout.vue';
import HomeView from '@/pages/HomeView.vue';
import LoginView from '@/pages/LoginView.vue';
import RegisterView from '@/pages/RegisterView.vue';
// import ForgotPasswordPage from '@/pages/ForgotPasswordPage.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '',
          name: 'home',
          component: HomeView,
        },
        {
          path: 'login',
          name: 'login',
          component: LoginView,
          meta: { guestOnly: true }
        },
        {
          path: 'register',
          name: 'register',
          component: RegisterView,
          meta: { guestOnly: true }
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
