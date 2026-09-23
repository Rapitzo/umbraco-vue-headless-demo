<script setup lang="ts">
import { firstImage } from '../../api/media'
import type { ImageBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'

defineProps<{ block: ImageBlock }>()
</script>

<template>
  <figure v-if="firstImage(block.properties.image)" class="image-block">
    <ResponsiveImage
      :media="firstImage(block.properties.image)!"
      preset="standard"
      sizes="(min-width: 76rem) 1200px, 100vw"
    />
    <figcaption v-if="block.properties.caption">{{ block.properties.caption }}</figcaption>
  </figure>
</template>

<style scoped>
.image-block {
  margin: 0;
  container-type: inline-size;
}

.image-block img {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

/* Wide placements get a wider crop so the image does not dominate the page. */
@container (min-width: 60rem) {
  .image-block img {
    aspect-ratio: 16 / 9;
  }
}

figcaption {
  margin-top: var(--space-xs);
  max-width: 48ch;
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}
</style>
