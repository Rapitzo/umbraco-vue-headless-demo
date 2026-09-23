import { createRouter, createWebHistory } from 'vue-router'
import ArticleListPage from './pages/ArticleListPage.vue'
import ArticlePage from './pages/ArticlePage.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: ArticleListPage },
    // Any other path is resolved against the Umbraco content tree.
    { path: '/:path(.*)*', component: ArticlePage },
  ],
})
