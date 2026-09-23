<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getNewsItems } from '../api/client'
import type { NewsListPage, NewsTeaser } from '../api/types'
import NewsLead from '../components/NewsLead.vue'
import NewsRow from '../components/NewsRow.vue'
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
  <div class="news page-grid">
    <p v-if="error" class="status" role="alert">The news could not be loaded. Please try again later.</p>
    <p v-else-if="!items" class="status" aria-live="polite">Loading news…</p>
    <!-- On a fresh install Umbraco builds the Delivery API index in the background. -->
    <p v-else-if="items.length === 0" class="status">No news yet. Check back soon.</p>
    <template v-else>
      <NewsLead :item="items[0]!" class="news__lead" />
      <section v-if="items.length > 1" class="news__index" aria-labelledby="news-earlier">
        <h2 id="news-earlier" class="label">Earlier</h2>
        <NewsRow v-for="item in items.slice(1)" :key="item.id" :item="item" />
      </section>
    </template>
  </div>
</template>

<style scoped>
.news {
  row-gap: var(--space-3xl);
}

.news__lead {
  grid-column: full;
}

.news__index .label {
  padding-bottom: var(--space-xs);
  border-bottom: var(--rule-ink);
}
</style>
