<script setup lang="ts">
import { firstImage } from '../../api/media'
import type { CardBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'

defineProps<{ block: CardBlock }>()
</script>

<template>
  <article class="tile">
    <div v-if="firstImage(block.properties.image)" class="tile__frame">
      <ResponsiveImage
        class="tile__image"
        :media="firstImage(block.properties.image)!"
        preset="original"
        sizes="(min-width: 48rem) 45vw, 100vw"
      />
    </div>
    <h3 class="tile__title">
      <RouterLink v-if="block.properties.link" :to="block.properties.link.route.path" class="tile__link">
        {{ block.properties.title }}
      </RouterLink>
      <template v-else>{{ block.properties.title }}</template>
    </h3>
    <p v-if="block.properties.text" class="tile__text">{{ block.properties.text }}</p>
  </article>
</template>

<style scoped>
.tile {
  position: relative;
}

.tile__frame {
  overflow: hidden;
  aspect-ratio: var(--tile-ratio, 4 / 3);
}

.tile__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform var(--dur-long) var(--ease-out);
}

.tile__title {
  margin-top: var(--space-sm);
  font-size: var(--text-lg);
}

.tile__link {
  color: var(--color-ink);
  text-decoration: none;
}

.tile__link::after {
  content: '';
  position: absolute;
  inset: 0;
}

.tile__text {
  margin: var(--space-2xs) 0 0;
  max-width: 36ch;
  color: var(--color-muted);
}

.tile:hover .tile__image {
  transform: scale(1.03);
}

.tile:hover .tile__link {
  color: var(--color-accent-strong);
  text-decoration: underline;
  text-decoration-thickness: 2px;
}

.tile:has(.tile__link:focus-visible) {
  outline: 3px solid var(--color-focus);
  outline-offset: 6px;
}

.tile__link:focus-visible {
  outline: none;
}
</style>
