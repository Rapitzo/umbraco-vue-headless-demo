<script setup lang="ts">
import { ref, watch, type Component } from 'vue'
import { useRoute } from 'vue-router'
import { NotFoundError, getPageByPath } from '../api/client'
import type { Page } from '../api/types'
import { useSite } from '../composables/useSite'
import ContactPage from './ContactPage.vue'
import ContentPage from './ContentPage.vue'
import HomePage from './HomePage.vue'
import NewsItemPage from './NewsItemPage.vue'
import NewsListPage from './NewsListPage.vue'
import NotFoundPage from './NotFoundPage.vue'

// Document type alias -> page component. The CMS owns the URL; the content type picks the template.
const templates: Record<Page['contentType'], Component> = {
  home: HomePage,
  contentPage: ContentPage,
  newsList: NewsListPage,
  newsItem: NewsItemPage,
  contactPage: ContactPage,
}

const route = useRoute()
const { settings, pageType } = useSite()
const page = ref<Page | null>(null)
const notFound = ref(false)
const error = ref<string | null>(null)

watch(
  () => route.path,
  async (path) => {
    notFound.value = false
    error.value = null
    try {
      const result = await getPageByPath(path)
      if (!Object.hasOwn(templates, result.contentType)) throw new NotFoundError(path)
      page.value = result
      pageType.value = result.contentType
    } catch (e) {
      page.value = null
      pageType.value = null
      if (e instanceof NotFoundError) notFound.value = true
      else error.value = e instanceof Error ? e.message : String(e)
    }
  },
  { immediate: true },
)

// Title and meta description follow the page, falling back to the site settings.
watch([page, notFound, settings], () => {
  const siteName = settings.value?.siteName ?? ''
  const title = notFound.value ? 'Page not found' : page.value?.contentType === 'home' ? '' : page.value?.name
  document.title = title ? `${title} | ${siteName}` : [siteName, settings.value?.tagline].filter(Boolean).join(', ')
  document.querySelector('meta[name="description"]')?.setAttribute('content', page.value?.properties.metaDescription ?? '')
})
</script>

<template>
  <NotFoundPage v-if="notFound" />
  <div v-else-if="error" class="page-grid">
    <p class="status" role="alert">Sorry, this page could not be loaded. ({{ error }})</p>
  </div>
  <component :is="templates[page.contentType]" v-else-if="page" :key="page.id" :page="page" />
  <div v-else class="page-grid">
    <p class="status" aria-live="polite">Loading…</p>
  </div>
</template>
