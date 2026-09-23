<script setup lang="ts">
import type { QuoteBlock } from '../../api/types'

defineProps<{ block: QuoteBlock }>()
</script>

<template>
  <figure class="quote">
    <blockquote>
      <p>{{ block.properties.quote }}</p>
    </blockquote>
    <figcaption v-if="block.properties.attribution">{{ block.properties.attribution }}</figcaption>
  </figure>
</template>

<style scoped>
.quote {
  margin: 0;
  padding-block: clamp(1.5rem, 4vw, 3rem);
  border-block: 1px solid var(--line);
  text-align: center;
}

blockquote {
  margin: 0;
}

blockquote p {
  max-width: 20em;
  margin: 0 auto;
  font-family: var(--font-serif);
  font-style: italic;
  font-size: clamp(1.75rem, 4vw, 2.75rem);
  line-height: 1.2;
}

figcaption {
  margin-top: 1rem;
  font-size: 0.875rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--ink-muted);
}
</style>
