<script setup lang="ts">
import { firstImage } from '../api/media'
import type { NewsTeaser } from '../api/types'
import DateMark from './DateMark.vue'
import ResponsiveImage from './ResponsiveImage.vue'

// One line of the news index: date, headline and teaser, small photo at the end.
defineProps<{ item: NewsTeaser }>()
</script>

<template>
  <article class="news-row">
    <DateMark class="news-row__date" :value="item.properties.publishDate" />
    <div class="news-row__text">
      <h3 class="news-row__title">
        <RouterLink :to="item.route.path" class="news-row__link">{{ item.name }}</RouterLink>
      </h3>
      <p>{{ item.properties.teaser }}</p>
    </div>
    <ResponsiveImage
      v-if="firstImage(item.properties.image)"
      class="news-row__image"
      :media="firstImage(item.properties.image)!"
      preset="square"
      sizes="(min-width: 48rem) 240px, 30vw"
    />
  </article>
</template>

<style scoped>
.news-row {
  position: relative;
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(0, 6rem);
  gap: var(--space-sm) var(--space-md);
  padding-block: var(--space-lg);
  border-bottom: var(--rule);
}

.news-row__date {
  grid-column: 1 / -1;
}

.news-row__title {
  font-size: var(--text-lg);
  line-height: 1;
}

.news-row__link {
  color: var(--color-ink);
  text-decoration: none;
}

.news-row__link::after {
  content: '';
  position: absolute;
  inset: 0;
}

.news-row:hover .news-row__link {
  color: var(--color-accent-strong);
  text-decoration: underline;
  text-decoration-thickness: 2px;
}

.news-row p {
  margin: var(--space-xs) 0 0;
  max-width: 52ch;
  color: var(--color-muted);
}

.news-row__image {
  width: 100%;
  aspect-ratio: 1;
  object-fit: cover;
}

@media (min-width: 48rem) {
  .news-row {
    grid-template-columns: minmax(0, 2fr) minmax(0, 7fr) minmax(0, 3fr);
    column-gap: var(--gutter);
    align-items: start;
  }

  .news-row__date {
    grid-column: auto;
  }

  .news-row__image {
    max-width: 14rem;
    justify-self: end;
  }
}
</style>
