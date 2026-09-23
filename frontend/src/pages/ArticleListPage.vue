<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getArticleList, getArticles } from '../api/client'
import type { Article, ArticleList } from '../api/types'

const page = ref<ArticleList | null>(null)
const articles = ref<Article[]>([])
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    ;[page.value, articles.value] = await Promise.all([getArticleList(), getArticles()])
  } catch (e) {
    error.value = e instanceof Error ? e.message : String(e)
  }
})
</script>

<template>
  <p v-if="error" class="status error">Could not load content: {{ error }}</p>
  <p v-else-if="!page" class="status">Loading…</p>
  <template v-else>
    <header class="page-header">
      <h1>{{ page.name }}</h1>
      <p v-if="page.properties.intro" class="lead">{{ page.properties.intro }}</p>
    </header>

    <p v-if="articles.length === 0" class="status">
      No articles in the index yet. On first boot Umbraco builds the Delivery API index in the background; reload in a minute.
    </p>
    <ul v-else class="article-list">
      <li v-for="article in articles" :key="article.id">
        <RouterLink :to="article.route.path" class="card">
          <h2>{{ article.name }}</h2>
          <p v-if="article.properties.summary">{{ article.properties.summary }}</p>
        </RouterLink>
      </li>
    </ul>
  </template>
</template>
