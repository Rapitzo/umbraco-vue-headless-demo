<script setup lang="ts">
import type { BlockGridArea, CardRowBlock } from '../../api/types'
import BlockGrid from '../BlockGrid.vue'

// The cards live in the block's "cards" area, so they render through the same grid.
defineProps<{ block: CardRowBlock; areas?: BlockGridArea[]; areaGridColumns?: number | null }>()
</script>

<template>
  <section class="card-row">
    <h2 v-if="block.properties.heading">{{ block.properties.heading }}</h2>
    <BlockGrid v-for="area in areas" :key="area.alias" :model="{ gridColumns: areaGridColumns ?? 12, items: area.items }" />
  </section>
</template>

<style scoped>
.card-row h2 {
  margin-bottom: 1.5rem;
}

/* Cards in a row share the same height. */
.card-row :deep(.block-grid) {
  align-items: stretch;
  row-gap: 1.5rem;
}
</style>
