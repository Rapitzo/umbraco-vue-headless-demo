<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { NotFoundError, getArticleByPath } from '../api/client'
import type { Article } from '../api/types'
import BlockList from '../components/BlockList.vue'

const route = useRoute()
const article = ref<Article | null>(null)
const error = ref<string | null>(null)

// The frontend URL mirrors the Umbraco route, so the CMS stays in charge of URLs.
watch(
  () => route.path,
  async (path) => {
    article.value = null
    error.value = null
    try {
      const content = await getArticleByPath(path.endsWith('/') ? path : `${path}/`)
      if (content.contentType !== 'article') throw new NotFoundError(path)
      article.value = content
    } catch (e) {
      error.value = e instanceof NotFoundError ? 'Article not found.' : `Could not load content: ${(e as Error).message}`
    }
  },
  { immediate: true },
)
</script>

<template>
  <RouterLink to="/" class="back">← All articles</RouterLink>
  <p v-if="error" class="status error">{{ error }}</p>
  <p v-else-if="!article" class="status">Loading…</p>
  <article v-else>
    <header class="page-header">
      <h1>{{ article.name }}</h1>
      <p v-if="article.properties.summary" class="lead">{{ article.properties.summary }}</p>
      <time :datetime="article.updateDate">Updated {{ new Date(article.updateDate).toLocaleDateString() }}</time>
    </header>
    <BlockList :model="article.properties.body" />
  </article>
</template>
