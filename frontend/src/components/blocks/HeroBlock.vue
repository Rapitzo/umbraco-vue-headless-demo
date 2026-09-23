<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../../api/media'
import type { HeroBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'

const props = defineProps<{ block: HeroBlock }>()
const image = computed(() => firstImage(props.block.properties.image))
</script>

<template>
  <section class="hero">
    <div v-if="image" class="hero__media">
      <!-- A tall crop on phones shows the image at up to ~2.2x the viewport width. -->
      <ResponsiveImage
        class="hero__image"
        :media="image"
        preset="original"
        sizes="(max-width: 48rem) 220vw, 100vw"
        eager
      />
    </div>
    <h2 class="hero__heading" :class="{ 'hero__heading--plain': !image }">{{ block.properties.heading }}</h2>
    <div class="hero__aside">
      <p v-if="block.properties.text" class="lead">{{ block.properties.text }}</p>
      <RouterLink
        v-if="block.properties.ctaLink && block.properties.ctaLabel"
        :to="block.properties.ctaLink.route.path"
        class="link-cta"
      >
        {{ block.properties.ctaLabel }}
      </RouterLink>
    </div>
  </section>
</template>

<style scoped>
.hero {
  display: grid;
  grid-template-columns: subgrid;
}

.hero__media {
  grid-column: full;
  grid-row: 1;
  position: relative;
  height: clamp(30rem, 82vh, 54rem);
}

.hero__media::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, var(--color-scrim), transparent 60%);
}

.hero__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.hero__heading {
  grid-column: content;
  grid-row: 1;
  align-self: end;
  position: relative;
  z-index: 1;
  max-width: 9ch;
  padding-bottom: var(--space-xl);
  font-size: var(--text-display);
  color: var(--color-on-image);
  animation: rise var(--dur-long) var(--ease-out) both;
}

.hero__heading--plain {
  color: var(--color-ink);
}

.hero__aside {
  grid-column: content;
  padding-top: var(--space-xl);
}

@media (min-width: 48rem) {
  .hero__aside {
    grid-column: 8 / 14;
  }
}

@keyframes rise {
  from {
    opacity: 0;
    transform: translateY(0.25em);
  }
}
</style>
