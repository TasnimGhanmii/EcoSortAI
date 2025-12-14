import { createRouter, createWebHistory } from 'vue-router'
import Auth from '../pages/auth.vue'
import Dashboard from '../pages/Dashboard.vue'

const routes = [
  { path: '/', redirect: '/auth' },
  { 
    path: '/auth', 
    name: 'Auth', 
    component: Auth,
    meta: { guestOnly: true } // Only accessible if NOT logged in
  },
  { 
    path: '/dashboard', 
    name: 'Dashboard', 
    component: Dashboard,
    meta: { requiresAuth: true } // Only accessible if logged in
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Global navigation guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')

  if (to.meta.requiresAuth && !token) {
    // Trying to access dashboard without being logged in
    next('/auth')
  } else if (to.meta.guestOnly && token) {
    // Logged-in user trying to access login/register page
    next('/dashboard')
  } else {
    next()
  }
})

export default router
