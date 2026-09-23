<script setup lang="ts">
import { computed } from 'vue'
import type { BlockGridArea, CardBlock as Card, CardRowBlock } from '../../api/types'
import CardBlock from './CardBlock.vue'

// The cards live in the block's "cards" area. The row sets them as staggered tiles of
// different sizes rather than equal columns, so it renders them itself.
const props = defineProps<{ block: CardRowBlock; areas?: BlockGridArea[] }>()

const cards = computed(() =>
  (props.areas ?? [])
    .flatMap((area) => area.items)
    .map((item) => item.content)
    .filter((content): content is Card => content.contentType === 'cardBlock'),
)
</script>

<template>
  <section class="tiles">
    <h2 v-if="block.properties.heading" class="tiles__heading">{{ block.properties.heading }}</h2>
    <ul class="tiles__list">
      <li v-for="card in cards" :key="card.id" class="tiles__item">
        <CardBlock :block="card" />
      </li>
    </ul>
  </section>
</template>

<style scoped>
.tiles {
  display: grid;
  gap: var(--space-lg);
}

.tiles__heading {
  font-size: var(--text-xl);
  max-width: 8ch;
}

.tiles__list {
  display: grid;
  gap: var(--space-xl);
  margin: 0;
  padding: 0;
  list-style: none;
}

/* Desktop: a tall tile on the left; the heading, a wide tile and a small one step down
   the right. Tiles past the third fall back to half width. */
@media (min-width: 48rem) {
  .tiles {
    grid-template-columns: repeat(12, minmax(0, 1fr));
    column-gap: var(--gutter);
    row-gap: var(--space-xl);
  }

  .tiles__list {
    display: contents;
  }

  .tiles__item {
    grid-column: span 6;
  }

  .tiles__heading {
    grid-column: 7 / 13;
    grid-row: 1;
    align-self: end;
    font-size: var(--text-display);
    max-width: 9ch;
  }

  .tiles__item:nth-child(1) {
    grid-column: 1 / 6;
    grid-row: 1 / 4;
    --tile-ratio: 4 / 5;
  }

  .tiles__item:nth-child(2) {
    grid-column: 7 / 13;
    grid-row: 2;
  }

  .tiles__item:nth-child(3) {
    grid-column: 8 / 12;
    grid-row: 3;
    --tile-ratio: 1 / 1;
  }
}
</style>
