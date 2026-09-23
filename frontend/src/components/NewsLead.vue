<script setup lang="ts">
import { firstImage } from '../api/media'
import type { NewsTeaser } from '../api/types'
import DateMark from './DateMark.vue'
import ResponsiveImage from './ResponsiveImage.vue'

// The newest story, set large at the top of the news page.
defineProps<{ item: NewsTeaser }>()
</script>

<template>
  <article class="lead-story">
    <ResponsiveImage
      v-if="firstImage(item.properties.image)"
      class="lead-story__image"
      :media="firstImage(item.properties.image)!"
      preset="standard"
      sizes="(min-width: 48rem) 60vw, 100vw"
      eager
    />
    <div class="lead-story__text">
      <DateMark :value="item.properties.publishDate" size="lg" />
      <h2 class="lead-story__title">
        <RouterLink :to="item.route.path" class="lead-story__link">{{ item.name }}</RouterLink>
      </h2>
      <p class="lead-story__teaser">{{ item.properties.teaser }}</p>
    </div>
  </article>
</template>

<style scoped>
.lead-story {
  position: relative;
  display: grid;
  grid-template-columns: subgrid;
  row-gap: var(--space-lg);
}

.lead-story__image {
  grid-column: full;
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

.lead-story__text {
  grid-column: content;
}

.lead-story__title {
  margin-top: var(--space-md);
  font-size: var(--text-xl);
}

.lead-story__link {
  color: var(--color-ink);
  text-decoration: none;
}

.lead-story__link::after {
  content: '';
  position: absolute;
  inset: 0;
}

.lead-story:hover .lead-story__link {
  color: var(--color-accent-strong);
  text-decoration: underline;
  text-decoration-thickness: 3px;
}

.lead-story__teaser {
  margin: var(--space-md) 0 0;
  max-width: 36ch;
  font-size: var(--text-md);
  line-height: 1.4;
}

/* Desktop: photo bleeds off the left edge, the story sits in the last four columns. */
@media (min-width: 48rem) {
  .lead-story__image {
    grid-column: full-start / 10;
  }

  .lead-story__text {
    grid-column: 10 / 14;
    align-self: end;
  }
}
</style>
