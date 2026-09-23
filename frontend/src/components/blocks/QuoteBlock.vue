<script setup lang="ts">
import type { QuoteBlock } from '../../api/types'

defineProps<{ block: QuoteBlock }>()
</script>

<template>
  <figure class="quote">
    <blockquote>
      <p>{{ block.properties.quote }}</p>
    </blockquote>
    <figcaption v-if="block.properties.attribution" class="label">{{ block.properties.attribution }}</figcaption>
  </figure>
</template>

<style scoped>
.quote {
  margin: 0;
  padding-top: var(--space-lg);
  border-top: var(--rule-ink);
}

blockquote {
  margin: 0;
}

blockquote p {
  margin: 0;
  max-width: 24ch;
  text-wrap: balance;
  font-family: var(--font-display);
  font-size: var(--text-xl);
  font-weight: 400;
  line-height: 1;
  letter-spacing: var(--tracking-display);
  text-indent: -0.4em;
}

blockquote p::before {
  content: '“';
  color: var(--color-accent);
}

blockquote p::after {
  content: '”';
  color: var(--color-accent);
}

figcaption {
  margin-top: var(--space-md);
}
</style>
