<script setup lang="ts">
import { computed } from 'vue'
import type { BakeBoardBlock } from '../../api/types'

const props = defineProps<{ block: BakeBoardBlock }>()

const bakes = computed(() => props.block.properties.items?.items.map((item) => item.content) ?? [])
const headingId = computed(() => `bake-board-${props.block.id}`)
</script>

<template>
  <section class="board" :aria-labelledby="headingId">
    <div class="board__head">
      <h2 :id="headingId" class="board__heading">{{ block.properties.heading }}</h2>
      <p v-if="block.properties.intro" class="board__intro">{{ block.properties.intro }}</p>
    </div>

    <table v-if="bakes.length" class="board__list">
      <caption class="visually-hidden">{{ block.properties.heading }}: bake, time out of the oven and price</caption>
      <thead>
        <tr>
          <th scope="col" class="label">Bake</th>
          <td class="board__leader" aria-hidden="true" />
          <th scope="col" class="label board__num"><span lang="sv">Ur ugnen</span><span class="visually-hidden"> (out of the oven)</span></th>
          <th scope="col" class="label board__num">SEK</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="bake in bakes" :key="bake.id" :class="{ 'is-sold-out': bake.properties.soldOut }">
          <th scope="row" class="board__name">
            <span class="board__title">{{ bake.properties.name }}</span>
            <span v-if="bake.properties.note" class="board__note">{{ bake.properties.note }}</span>
          </th>
          <td class="board__leader" aria-hidden="true" />
          <td class="board__num board__time">{{ bake.properties.readyAt }}</td>
          <td class="board__num board__price">
            <span v-if="bake.properties.soldOut" class="board__stamp">Sold out</span>
            <template v-else>{{ bake.properties.price }}</template>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<style scoped>
.board {
  display: grid;
  grid-template-columns: subgrid;
  row-gap: var(--space-xl);
  padding-block: var(--space-2xl) var(--space-3xl);
  background: var(--color-paper-2);
  border-block: var(--rule-ink);
}

.board__head {
  grid-column: content;
}

.board__heading {
  font-size: var(--text-display);
  line-height: 0.85;
}

.board__intro {
  margin-top: var(--space-md);
  max-width: 28ch;
  font-style: italic;
  color: var(--color-muted);
}

.board__list {
  grid-column: content;
  width: 100%;
  border-collapse: collapse;
  font-variant-numeric: tabular-nums lining-nums;
}

.board__list thead th {
  padding-bottom: var(--space-xs);
  text-align: left;
  border-bottom: var(--rule-ink);
}

.board__list thead td {
  border-bottom: var(--rule-ink);
  background: none;
}

.board__list tbody tr {
  border-bottom: var(--rule);
}

.board__list th,
.board__list td {
  padding-block: var(--space-sm);
  vertical-align: baseline;
}

.board__name {
  text-align: left;
  font-weight: inherit;
}

.board__title {
  display: block;
  font-family: var(--font-display);
  font-size: var(--text-md);
  font-weight: 700;
  font-stretch: 85%;
  letter-spacing: -0.015em;
  line-height: 1.1;
}

.board__note {
  display: block;
  margin-top: var(--space-3xs);
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}

/* Dot leaders run from the name to the numbers, like a printed price list.
   On phones there is no room for them, so the columns simply sit apart. */
.board__leader {
  display: none;
  width: 100%;
  min-width: var(--space-lg);
  background-image: radial-gradient(circle, var(--color-rule) 1.2px, transparent 1.6px);
  background-size: 0.5em 0.5em;
  background-repeat: repeat-x;
  background-position: 0 70%;
}

.board__num {
  padding-left: var(--space-xs);
  text-align: right;
}

tbody .board__num {
  white-space: nowrap;
}

.board__time {
  font-size: var(--text-sm);
  color: var(--color-muted);
}

.board__price {
  min-width: 4.5ch;
  font-family: var(--font-display);
  font-size: var(--text-md);
  font-weight: 700;
  font-stretch: 85%;
}

.is-sold-out .board__title {
  text-decoration: line-through;
  text-decoration-thickness: 2px;
  text-decoration-color: var(--color-accent);
  color: var(--color-muted);
}

.board__stamp {
  display: inline-block;
  padding: var(--space-3xs) var(--space-2xs);
  border: 2px solid var(--color-accent);
  font-size: var(--text-label);
  letter-spacing: 0.06em;
  line-height: 1.1;
  white-space: normal;
  text-align: center;
  text-transform: uppercase;
  color: var(--color-accent);
  transform: rotate(-3deg);
}

@media (min-width: 64rem) {
  .board__head {
    grid-column: 2 / 6;
  }

  .board__list {
    grid-column: 7 / 14;
  }

  .board__leader {
    display: table-cell;
  }

  .board__num {
    padding-left: var(--space-md);
  }

  /* Names keep their own width; the dot leaders take up the slack. */
  .board__name {
    width: 1%;
    white-space: nowrap;
  }
}
</style>
