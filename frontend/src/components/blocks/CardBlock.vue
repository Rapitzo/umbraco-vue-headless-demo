<script setup lang="ts">
import { firstImage } from '../../api/media'
import type { CardBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'

defineProps<{ block: CardBlock }>()
</script>

<template>
  <article class="card">
    <ResponsiveImage
      v-if="firstImage(block.properties.image)"
      class="card__image"
      :media="firstImage(block.properties.image)!"
      preset="standard"
      sizes="(min-width: 48rem) 400px, 100vw"
    />
    <div class="card__body">
      <h3 class="card__title">
        <RouterLink v-if="block.properties.link" :to="block.properties.link.route.path" class="card__link">
          {{ block.properties.title }}
        </RouterLink>
        <template v-else>{{ block.properties.title }}</template>
      </h3>
      <p v-if="block.properties.text">{{ block.properties.text }}</p>
    </div>
  </article>
</template>

<style scoped>
.card {
  position: relative;
  height: 100%;
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  overflow: hidden;
  transition: border-color 150ms ease;
}

.card:hover {
  border-color: var(--rye);
}

.card__image {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

.card__body {
  padding: 1.25rem 1.5rem 1.5rem;
}

.card__body p {
  margin: 0;
  color: var(--ink-muted);
}

.card__link {
  color: inherit;
  text-decoration: none;
}

/* The whole card is clickable, but only the title is the accessible link. */
.card__link::after {
  content: '';
  position: absolute;
  inset: 0;
}

.card:hover .card__link {
  text-decoration: underline;
}

.card:has(.card__link:focus-visible) {
  outline: 3px solid var(--focus);
  outline-offset: 3px;
}

.card__link:focus-visible {
  outline: none;
}
</style>
