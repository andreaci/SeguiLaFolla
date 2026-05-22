import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: () => import('../views/HomeView.vue') },
    { path: '/accedi', name: 'login', component: () => import('../views/LoginView.vue') },
    { path: '/registrati', name: 'register', component: () => import('../views/RegisterView.vue') },
    { path: '/direttore', name: 'director-home', component: () => import('../views/DirectorHomeView.vue') },
    { path: '/direttore/:id', name: 'director', component: () => import('../views/DirectorView.vue') },
    { path: '/partita/:id', name: 'play', component: () => import('../views/PlayView.vue') },
    { path: '/entra/:id', name: 'join', component: () => import('../views/JoinView.vue') },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()
  if (!auth.user && localStorage.getItem('mandria_token')) {
    await auth.loadMe()
  }
  if (to.meta.auth && !auth.isLoggedIn) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
})

export default router
