import { createRouter, createWebHistory } from 'vue-router'
import PageResolver from './pages/PageResolver.vue'

// One catch-all route: every path is looked up in the Umbraco content tree, and the
// content type of the result decides which page component renders (see PageResolver.vue).
export const router = createRouter({
  history: createWebHistory(),
  routes: [{ path: '/:path(.*)*', component: PageResolver }],
  scrollBehavior: (to, _from, saved) => saved ?? (to.hash ? { el: to.hash } : { top: 0 }),
})
