<script setup lang="ts">
import { firstImage } from '../../api/media'
import type { HeroBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'

defineProps<{ block: HeroBlock }>()
</script>

<template>
  <section class="hero">
    <div class="hero__text">
      <h2 class="hero__heading">{{ block.properties.heading }}</h2>
      <p v-if="block.properties.text" class="lead">{{ block.properties.text }}</p>
      <RouterLink
        v-if="block.properties.ctaLink && block.properties.ctaLabel"
        :to="block.properties.ctaLink.route.path"
        class="button"
      >
        {{ block.properties.ctaLabel }}
      </RouterLink>
    </div>
    <ResponsiveImage
      v-if="firstImage(block.properties.image)"
      class="hero__image"
      :media="firstImage(block.properties.image)!"
      preset="standard"
      sizes="(min-width: 64rem) 680px, 100vw"
      eager
    />
  </section>
</template>

<style scoped>
.hero {
  display: grid;
  gap: 2rem;
  align-items: center;
}

.hero__heading {
  font-size: clamp(2.5rem, 6vw, 4.5rem);
  line-height: 1.02;
  max-width: 11ch;
}

.hero__text .lead {
  margin-bottom: 1.75rem;
}

.hero__image {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
  border-radius: var(--radius);
}

@media (min-width: 64rem) {
  .hero {
    grid-template-columns: 5fr 7fr;
    gap: 4rem;
  }
}

@media (max-width: 63.99rem) {
  .hero__image {
    order: -1;
  }
}
</style>
