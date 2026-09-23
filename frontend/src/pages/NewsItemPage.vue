<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../api/media'
import type { NewsItemPage } from '../api/types'
import BlockGrid from '../components/BlockGrid.vue'
import PageHeader from '../components/PageHeader.vue'
import ResponsiveImage from '../components/ResponsiveImage.vue'
import { formatDate } from '../utils/formatDate'

const props = defineProps<{ page: NewsItemPage }>()

const image = computed(() => firstImage(props.page.properties.image))
// The parent route is the news list, whatever editors have named it.
const listPath = computed(() => props.page.route.path.replace(/[^/]+\/$/, ''))
</script>

<template>
  <article>
    <PageHeader :title="page.name" :intro="page.properties.teaser">
      <p class="meta">
        <time :datetime="page.properties.publishDate">{{ formatDate(page.properties.publishDate) }}</time>
        <span aria-hidden="true"> · </span>
        <RouterLink :to="listPath">All news</RouterLink>
      </p>
    </PageHeader>
    <div class="container">
      <ResponsiveImage
        v-if="image"
        class="news-image"
        :media="image"
        preset="wide"
        sizes="(min-width: 75rem) 1200px, 100vw"
        eager
      />
      <BlockGrid :model="page.properties.blocks" />
    </div>
  </article>
</template>

<style scoped>
.meta {
  margin: 1.5rem 0 0;
  color: var(--ink-muted);
}

.news-image {
  width: 100%;
  aspect-ratio: 16 / 9;
  object-fit: cover;
  border-radius: var(--radius);
  margin-bottom: clamp(2rem, 5vw, 4rem);
}
</style>
