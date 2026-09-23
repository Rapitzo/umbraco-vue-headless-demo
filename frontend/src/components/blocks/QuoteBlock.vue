<script setup lang="ts">
import type { QuoteBlock } from '../../api/types'

defineProps<{ block: QuoteBlock }>()
</script>

<template>
  <figure class="quote-block">
    <blockquote>{{ block.properties.quote }}</blockquote>
    <figcaption v-if="block.properties.attribution">{{ block.properties.attribution }}</figcaption>
  </figure>
</template>
