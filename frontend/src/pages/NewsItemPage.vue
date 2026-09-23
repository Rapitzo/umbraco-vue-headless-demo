<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../api/media'
import type { NewsItemPage } from '../api/types'
import BlockGrid from '../components/BlockGrid.vue'
import DateMark from '../components/DateMark.vue'
import PageHeader from '../components/PageHeader.vue'
import ResponsiveImage from '../components/ResponsiveImage.vue'

const props = defineProps<{ page: NewsItemPage }>()

const image = computed(() => firstImage(props.page.properties.image))
// The parent route is the news list, whatever editors have named it.
const listPath = computed(() => props.page.route.path.replace(/[^/]+\/$/, ''))
</script>

<template>
  <article class="news-item">
    <PageHeader :title="page.name" :intro="page.properties.teaser">
      <template #meta>
        <DateMark :value="page.properties.publishDate" />
      </template>
    </PageHeader>
    <div v-if="image" class="page-grid">
      <ResponsiveImage class="news-item__image" :media="image" preset="wide" sizes="100vw" eager />
    </div>
    <BlockGrid class="news-item__body" :model="page.properties.blocks" />
    <p class="news-item__back page-grid">
      <RouterLink :to="listPath" class="link-cta">More from the bakery</RouterLink>
    </p>
  </article>
</template>

<style scoped>
.news-item__image {
  grid-column: full;
  width: 100%;
  aspect-ratio: 16 / 9;
  max-height: 44rem;
  object-fit: cover;
}

.news-item__body {
  margin-top: var(--space-2xl);
}

.news-item__back {
  margin: var(--space-2xl) 0 0;
}
</style>
