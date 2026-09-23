<script setup lang="ts">
import { computed, type Component } from 'vue'
import type { BlockGridItem, BlockGridModel, GridBlock } from '../api/types'
import BakeBoardBlock from './blocks/BakeBoardBlock.vue'
import CardRowBlock from './blocks/CardRowBlock.vue'
import HeroBlock from './blocks/HeroBlock.vue'
import ImageBlock from './blocks/ImageBlock.vue'
import QuoteBlock from './blocks/QuoteBlock.vue'
import RichTextBlock from './blocks/RichTextBlock.vue'
import StoryBlock from './blocks/StoryBlock.vue'

const props = defineProps<{ model: BlockGridModel | null }>()

interface BlockView {
  component: Component
  /** Bleed to the page edges instead of sitting in the twelve content columns. */
  fullBleed?: boolean
}

// Element type alias -> component. The Record type makes the compiler flag a block type
// without a view. Blocks the frontend does not know are skipped, not rendered broken.
// Cards only appear inside a card row's area, which renders them itself.
const registry: Record<Exclude<GridBlock['contentType'], 'cardBlock'>, BlockView> = {
  heroBlock: { component: HeroBlock, fullBleed: true },
  bakeBoardBlock: { component: BakeBoardBlock, fullBleed: true },
  storyBlock: { component: StoryBlock, fullBleed: true },
  richTextBlock: { component: RichTextBlock },
  imageBlock: { component: ImageBlock },
  quoteBlock: { component: QuoteBlock },
  cardRowBlock: { component: CardRowBlock },
}

function viewFor(contentType: string): BlockView | undefined {
  return Object.hasOwn(registry, contentType) ? registry[contentType as keyof typeof registry] : undefined
}

// Umbraco packs grid items into rows left to right. Work out each item's start column the
// same way, so a 6 + 6 pair sits side by side and a lone 8 starts at the left edge.
const rows = computed(() => {
  const columns = props.model?.gridColumns ?? 12
  let cursor = 0
  return (props.model?.items ?? []).flatMap((item: BlockGridItem) => {
    const view = viewFor(item.content.contentType)
    if (!view) return []
    const span = Math.min(item.columnSpan, columns)
    if (cursor + span > columns) cursor = 0
    const start = cursor + 1
    cursor = (cursor + span) % columns
    // Line 1 is the left bleed track, so content column n starts at grid line n + 1.
    return [{ item, view, style: { '--col-start': start + 1, '--span': span } }]
  })
})
</script>

<template>
  <div v-if="rows.length" class="block-grid page-grid">
    <div
      v-for="{ item, view, style } in rows"
      :key="item.content.id"
      class="block-grid__item"
      :class="[`block-grid__item--${item.content.contentType}`, { 'block-grid__item--bleed': view.fullBleed }]"
      :style="style"
    >
      <component
        :is="view.component"
        :block="item.content"
        v-bind="item.areas.length ? { areas: item.areas } : {}"
      />
    </div>
  </div>
</template>

<style scoped>
.block-grid {
  row-gap: var(--space-3xl);
  align-items: start;
}

.block-grid__item {
  grid-column: content;
  min-width: 0;
}

.block-grid__item--bleed {
  grid-column: full;
  display: grid;
  grid-template-columns: subgrid;
}

/* Full-bleed blocks lay themselves out on the same columns through subgrid. */
.block-grid__item--bleed > :deep(*) {
  grid-column: 1 / -1;
}

@media (min-width: 48rem) {
  .block-grid__item:not(.block-grid__item--bleed) {
    grid-column: var(--col-start) / span var(--span);
  }
}
</style>
