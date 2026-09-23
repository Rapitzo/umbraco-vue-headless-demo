<script setup lang="ts">
import { computed, useId } from 'vue'
import { firstImage, imageUrl } from '../../api/media'
import type { BakeItemBlock } from '../../api/types'

// The closing scene of the cinema scroll: today's bakes on a shelf of cards, each photo
// standing on its card. They follow the slider in from the right, one after another.
// useCinemaScroll drives --board-* (heading) and --bakes-progress (cards).
const props = defineProps<{ heading: string | null; intro: string | null; bakes: BakeItemBlock[] }>()
const headingId = useId()
const title = computed(() => props.heading ?? 'Today’s bakes')

function photo(bake: BakeItemBlock): string | null {
  const media = firstImage(bake.properties.image)
  return media && imageUrl(media, 'square', 480)
}
</script>

<template>
  <section class="cinema-board" :aria-labelledby="headingId">
    <div class="cinema-board__head">
      <h2 :id="headingId" class="cinema-board__heading">{{ title }}</h2>
      <p v-if="intro" class="cinema-board__intro">{{ intro }}</p>
    </div>

    <ul class="cinema-board__shelf">
      <li
        v-for="(bake, i) in bakes"
        :key="bake.id"
        class="bake-card"
        :class="{ 'is-sold-out': bake.properties.soldOut, 'has-photo': photo(bake) }"
        :style="{ '--i': i, '--n': bakes.length }"
      >
        <img v-if="photo(bake)" class="bake-card__photo" :src="photo(bake)!" alt="" />
        <span class="bake-card__time">
          <template v-if="bake.properties.readyAt">
            <span lang="sv">Ur ugnen</span> <span class="bake-card__clock">{{ bake.properties.readyAt }}</span>
          </template>
        </span>
        <h3 class="bake-card__name">{{ bake.properties.name }}</h3>
        <p v-if="bake.properties.note" class="bake-card__note">{{ bake.properties.note }}</p>
        <div class="bake-card__foot">
          <span v-if="bake.properties.soldOut" class="bake-card__stamp">Sold out</span>
          <span v-else class="bake-card__price">{{ bake.properties.price }}<span class="bake-card__unit"> kr</span></span>
        </div>
      </li>
    </ul>
  </section>
</template>

<style scoped>
.cinema-board {
  --lift: clamp(64px, 9vh, 104px);

  position: absolute;
  left: 48px;
  right: 48px;
  top: 50%;
  z-index: 12;
  transform: translateY(-50%);
  visibility: var(--board-visibility);
}

.cinema-board__head {
  display: flex;
  flex-wrap: wrap;
  align-items: end;
  gap: var(--space-xs) var(--space-xl);
  opacity: var(--board-opacity);
  transform: translate3d(0, var(--board-y), 0);
}

.cinema-board__heading {
  color: var(--color-on-image);
  font-size: var(--text-display);
  line-height: 0.85;
  text-shadow: 0 16px 38px oklch(15% 0.02 50 / 0.32);
}

.cinema-board__intro {
  margin: 0;
  max-width: 30ch;
  color: var(--color-on-image);
  font-style: italic;
  line-height: 1.35;
  text-shadow: 0 2px 18px oklch(15% 0.02 50 / 0.45);
}

/* One row of six on wide screens. Each card spans four shared rows (time, name, note, price)
   through subgrid, so a name that wraps grows that row for every card beside it and the
   notes and footers stay level, with no space reserved when nothing wraps. */
.cinema-board__shelf {
  display: grid;
  grid-template-rows: repeat(4, auto);
  grid-auto-columns: minmax(0, 1fr);
  grid-auto-flow: column;
  gap: 0 clamp(12px, 1.15vw, 20px);
  margin: clamp(12px, 2vh, 24px) 0 0;
  padding: 0;
  list-style: none;
}

/* Each card gets its own slice of --bakes-progress, so they arrive one after another. */
.bake-card {
  --t: clamp(0, var(--bakes-progress) * 1.8 - var(--i) / max(var(--n) - 1, 1) * 0.8, 1);
  --e: calc(var(--t) * (2 - var(--t)));

  position: relative;
  display: grid;
  grid-row: span 4;
  grid-template-rows: subgrid;
  row-gap: 6px;
  margin-top: var(--lift);
  padding: 20px;
  border-radius: 24px;
  color: var(--color-ink);
  background: var(--color-paper);
  box-shadow: 0 18px 52px oklch(20% 0.04 50 / 0.16);
  opacity: var(--e);
  transform: translate3d(calc((1 - var(--e)) * 110vw), 0, 0);
  will-change: transform, opacity;
}

.bake-card.has-photo {
  padding-top: calc(var(--lift) * 1.25 + 8px);
}

/* The bake stands on the card: the cut-out breaks out of the top edge, with a soft contact
   shadow from its own alpha so it reads as sitting on the surface. */
.bake-card__photo {
  position: absolute;
  left: 50%;
  top: calc(var(--lift) * -1);
  width: auto;
  height: calc(var(--lift) * 2.3);
  max-width: 92%;
  object-fit: contain;
  transform: translateX(-50%);
  filter: drop-shadow(0 14px 12px oklch(22% 0.05 45 / 0.35));
}

.bake-card__name {
  align-self: end;
  font-size: clamp(1.1rem, 1.3vw + 0.1rem, var(--text-md));
  line-height: 1;
}

.bake-card__note {
  margin: 0 0 8px;
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}

.bake-card__foot {
  display: flex;
  align-items: end;
  justify-content: flex-start;
  gap: var(--space-xs);
  grid-row: 4;
  align-self: start;
  padding-top: 12px;
  border-top: var(--rule);
}

.bake-card__time {
  line-height: 1.25;
  font-family: var(--font-ui);
  font-size: var(--text-label);
  font-weight: 600;
  letter-spacing: var(--tracking-label);
  text-transform: uppercase;
  color: var(--color-muted);
}

.bake-card__clock {
  color: var(--color-ink);
}

.bake-card__price {
  font-family: var(--font-ui);
  font-size: var(--text-lg);
  font-weight: 700;
  line-height: 0.85;
  font-variant-numeric: tabular-nums lining-nums;
  white-space: nowrap;
}

.bake-card__unit {
  font-size: var(--text-sm);
  font-weight: 600;
  color: var(--color-muted);
}

.is-sold-out .bake-card__photo {
  filter: grayscale(0.7) opacity(0.55) drop-shadow(0 14px 12px oklch(22% 0.05 45 / 0.2));
}

.is-sold-out .bake-card__name {
  text-decoration: line-through;
  text-decoration-thickness: 2px;
  text-decoration-color: var(--color-accent);
  color: var(--color-muted);
}

.bake-card__stamp {
  flex: none;
  padding: 1px 5px;
  border: 2px solid var(--color-accent);
  font-size: 0.7rem;
  letter-spacing: 0.06em;
  line-height: 1.1;
  text-transform: uppercase;
  white-space: nowrap;
  color: var(--color-accent);
  transform: rotate(-3deg);
}

@media (max-width: 1100px) {
  .cinema-board__shelf {
    grid-template-rows: none;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    grid-auto-flow: row;
  }
}

@media (max-width: 800px) {
  .cinema-board {
    --lift: 36px;

    left: 16px;
    right: 16px;
  }

  .cinema-board__heading {
    font-size: var(--text-xl);
  }

  /* The prices already say kr; on a phone the shelf needs the room. */
  .cinema-board__intro {
    display: none;
  }

  .cinema-board__shelf {
    grid-template-columns: repeat(2, minmax(0, 1fr));
    column-gap: 10px;
  }

  .bake-card {
    padding: 12px;
    border-radius: 18px;
  }

  .bake-card.has-photo {
    padding-top: calc(var(--lift) * 1.25 + 6px);
  }

  .bake-card__name {
    font-size: 1.02rem;
  }

  .bake-card__note {
    font-size: 0.76rem;
  }

  .bake-card__foot {
    padding-top: 8px;
  }

  .bake-card__time {
    font-size: 0.6rem;
    letter-spacing: 0.06em;
  }

  .bake-card__price {
    font-size: 1.25rem;
  }
}

/* Short phones: drop the notes (on every card at once, so rows stay level) to fit the shelf. */
@media (max-width: 800px) and (max-height: 720px) {
  .cinema-board {
    --lift: 30px;
  }

  .bake-card__note {
    display: none;
  }
}
</style>
