<script setup lang="ts">
import { firstImage } from '../api/media'
import type { NewsTeaser } from '../api/types'
import { formatDate } from '../utils/formatDate'
import ResponsiveImage from './ResponsiveImage.vue'

defineProps<{ item: NewsTeaser; featured?: boolean }>()
</script>

<template>
  <article class="news-card" :class="{ 'news-card--featured': featured }">
    <ResponsiveImage
      v-if="firstImage(item.properties.image)"
      class="news-card__image"
      :media="firstImage(item.properties.image)!"
      preset="standard"
      :sizes="featured ? '(min-width: 64rem) 700px, 100vw' : '(min-width: 48rem) 400px, 100vw'"
      :eager="featured"
    />
    <div class="news-card__body">
      <time class="eyebrow" :datetime="item.properties.publishDate">{{ formatDate(item.properties.publishDate) }}</time>
      <h2 class="news-card__title">
        <RouterLink :to="item.route.path" class="news-card__link">{{ item.name }}</RouterLink>
      </h2>
      <p>{{ item.properties.teaser }}</p>
    </div>
  </article>
</template>

<style scoped>
.news-card {
  position: relative;
  display: grid;
  gap: 1.25rem;
  align-content: start;
}

.news-card__image {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
  border-radius: var(--radius);
}

.news-card .eyebrow {
  display: block;
}

.news-card__title {
  font-size: 1.5rem;
}

.news-card--featured .news-card__title {
  font-size: clamp(1.75rem, 3.5vw, 2.5rem);
}

.news-card p {
  margin: 0;
  color: var(--ink-muted);
}

.news-card__link {
  color: inherit;
  text-decoration: none;
}

.news-card__link::after {
  content: '';
  position: absolute;
  inset: 0;
}

.news-card:hover .news-card__link {
  text-decoration: underline;
}

@media (min-width: 64rem) {
  .news-card--featured {
    grid-template-columns: 7fr 5fr;
    gap: 3rem;
    align-items: center;
  }
}
</style>
