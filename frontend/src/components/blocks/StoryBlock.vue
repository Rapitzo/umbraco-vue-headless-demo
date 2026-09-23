<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../../api/media'
import type { StoryBlock } from '../../api/types'
import ResponsiveImage from '../ResponsiveImage.vue'
import RichTextContent from '../RichTextContent.vue'

const props = defineProps<{ block: StoryBlock }>()
const image = computed(() => firstImage(props.block.properties.image))
</script>

<template>
  <section class="story" :class="{ 'story--flip': block.properties.imageOnLeft }">
    <figure v-if="image" class="story__figure">
      <ResponsiveImage :media="image" preset="standard" sizes="(min-width: 48rem) 60vw, 100vw" />
      <figcaption v-if="block.properties.caption" class="story__caption">{{ block.properties.caption }}</figcaption>
    </figure>
    <div class="story__text">
      <h2 class="story__heading">{{ block.properties.heading }}</h2>
      <RichTextContent v-if="block.properties.text" :markup="block.properties.text.markup" />
    </div>
  </section>
</template>

<style scoped>
.story {
  display: grid;
  grid-template-columns: subgrid;
  row-gap: var(--space-lg);
}

.story__figure {
  grid-column: full;
  margin: 0;
}

.story__figure img {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

.story__caption {
  margin-top: var(--space-xs);
  padding-inline: var(--gutter);
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}

.story__text {
  grid-column: content;
}

.story__heading {
  font-size: var(--text-xl);
  margin-bottom: var(--space-md);
  max-width: 12ch;
}

/* Desktop: the photo runs off one edge of the page, the text sits low on the other side. */
@media (min-width: 48rem) {
  .story__figure {
    grid-column: 7 / full-end;
    grid-row: 1;
  }

  .story__caption {
    padding-inline: 0;
    max-width: 40ch;
  }

  .story__text {
    grid-column: 2 / 7;
    grid-row: 1;
    align-self: end;
    padding-bottom: var(--space-2xl);
  }

  .story--flip .story__figure {
    grid-column: full-start / 10;
  }

  .story--flip .story__caption {
    margin-left: auto;
    text-align: right;
  }

  .story--flip .story__text {
    grid-column: 10 / 14;
  }
}
</style>
