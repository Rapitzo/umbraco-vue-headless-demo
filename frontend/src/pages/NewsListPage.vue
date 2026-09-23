<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getNewsItems } from '../api/client'
import type { NewsListPage, NewsTeaser } from '../api/types'
import NewsCard from '../components/NewsCard.vue'
import PageHeader from '../components/PageHeader.vue'

const props = defineProps<{ page: NewsListPage }>()

const items = ref<NewsTeaser[] | null>(null)
const error = ref(false)

onMounted(async () => {
  try {
    items.value = await getNewsItems(props.page.id)
  } catch {
    error.value = true
  }
})
</script>

<template>
  <PageHeader :title="page.name" :intro="page.properties.intro" />
  <div class="container">
    <p v-if="error" class="status" role="alert">The news could not be loaded. Please try again later.</p>
    <p v-else-if="!items" class="status" aria-live="polite">Loading news…</p>
    <!-- On a fresh install Umbraco builds the Delivery API index in the background. -->
    <p v-else-if="items.length === 0" class="status">No news yet. Check back soon.</p>
    <template v-else>
      <NewsCard :item="items[0]!" featured class="news-featured" />
      <ul v-if="items.length > 1" class="news-grid">
        <li v-for="item in items.slice(1)" :key="item.id">
          <NewsCard :item="item" />
        </li>
      </ul>
    </template>
  </div>
</template>

<style scoped>
.news-featured {
  padding-bottom: clamp(2rem, 5vw, 3.5rem);
  margin-bottom: clamp(2rem, 5vw, 3.5rem);
  border-bottom: 1px solid var(--line);
}

.news-grid {
  display: grid;
  gap: 3rem 2rem;
  grid-template-columns: repeat(auto-fill, minmax(min(100%, 18rem), 1fr));
  margin: 0;
  padding: 0;
  list-style: none;
}
</style>
