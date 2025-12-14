import { createRouter, createWebHistory } from 'vue-router'
import Auth from '../pages/auth.vue'
import Dashboard from '../pages/Dashboard.vue'

const routes = [
  { path: '/', redirect: '/auth' },
  { path: '/auth', name: 'Auth', component: Auth },
  { path: '/dashboard', name: 'Dashboard', component: Dashboard }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
