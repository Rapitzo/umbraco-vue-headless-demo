<script setup lang="ts">
import type { Component } from 'vue'
import type { BlockGridModel, GridBlock } from '../api/types'
import CardBlock from './blocks/CardBlock.vue'
import CardRowBlock from './blocks/CardRowBlock.vue'
import HeroBlock from './blocks/HeroBlock.vue'
import ImageBlock from './blocks/ImageBlock.vue'
import QuoteBlock from './blocks/QuoteBlock.vue'
import RichTextBlock from './blocks/RichTextBlock.vue'

defineProps<{ model: BlockGridModel | null }>()

// Element type alias -> component. The Record type makes the compiler flag a block type
// that has no component. Blocks the frontend does not know are skipped, not rendered broken.
const registry: Record<GridBlock['contentType'], Component> = {
  heroBlock: HeroBlock,
  richTextBlock: RichTextBlock,
  imageBlock: ImageBlock,
  quoteBlock: QuoteBlock,
  cardRowBlock: CardRowBlock,
  cardBlock: CardBlock,
}

function componentFor(contentType: string): Component | undefined {
  return Object.hasOwn(registry, contentType) ? registry[contentType as GridBlock['contentType']] : undefined
}
</script>

<template>
  <div v-if="model?.items.length" class="block-grid" :style="{ '--grid-columns': model.gridColumns }">
    <template v-for="item in model.items" :key="item.content.id">
      <div
        v-if="componentFor(item.content.contentType)"
        class="block-grid__item"
        :class="`block-grid__item--${item.content.contentType}`"
        :style="{ '--span': item.columnSpan }"
      >
        <component
          :is="componentFor(item.content.contentType)"
          :block="item.content"
          v-bind="item.areas.length ? { areas: item.areas, areaGridColumns: item.areaGridColumns } : {}"
        />
      </div>
    </template>
  </div>
</template>

<style scoped>
.block-grid {
  display: grid;
  grid-template-columns: repeat(var(--grid-columns), minmax(0, 1fr));
  gap: clamp(2rem, 5vw, 4rem) clamp(1.5rem, 3vw, 2.5rem);
  align-items: center;
}

.block-grid__item {
  grid-column: 1 / -1;
  min-width: 0;
}

@media (min-width: 48rem) {
  .block-grid__item {
    grid-column: span var(--span);
  }
}
</style>
