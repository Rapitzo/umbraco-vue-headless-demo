<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import { firstImage, imageUrl } from '../../api/media'
import type { CinemaCardBlock } from '../../api/types'

const props = defineProps<{ cards: CinemaCardBlock[] }>()

// Infinite slider: three copies of the cards, starting on the middle one. When a move ends
// in an outer copy, jump back to the same card in the middle copy with the transition off.
// Only the middle copy is exposed to assistive tech and keyboard focus.
const looped = computed(() =>
  [0, 1, 2].flatMap((set) =>
    props.cards.map((card, i) => ({ card, index: set * props.cards.length + i, clone: set !== 1 })),
  ),
)
const track = ref<HTMLElement | null>(null)
const active = ref(props.cards.length)
const step = ref(0)
const jumping = ref(false)

function measure() {
  const first = track.value?.firstElementChild as HTMLElement | null
  if (!track.value || !first) return
  step.value = first.offsetWidth + parseFloat(getComputedStyle(track.value).columnGap || '0')
}

const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)')

function move(direction: number) {
  active.value += direction
  // Without the slide transition there is no transitionend to normalize on.
  if (reduceMotion.matches) normalize()
}

function normalize(event?: TransitionEvent) {
  if (event && event.target !== track.value) return
  const n = props.cards.length
  if (active.value >= n * 2) jump(active.value - n)
  else if (active.value < n) jump(active.value + n)
}

async function jump(index: number) {
  jumping.value = true
  active.value = index
  await nextTick()
  requestAnimationFrame(() => requestAnimationFrame(() => (jumping.value = false)))
}

onMounted(() => {
  measure()
  window.addEventListener('resize', measure)
})
onBeforeUnmount(() => window.removeEventListener('resize', measure))

defineExpose({ move })
</script>

<template>
  <section class="cinema__cards" aria-label="Highlights">
    <div
      ref="track"
      class="cinema__track"
      :class="{ 'is-jumping': jumping }"
      :style="{ '--cards-shift': `${-step * active}px` }"
      @transitionend="normalize"
    >
      <article
        v-for="{ card, index, clone } in looped"
        :key="index"
        class="cinema-card"
        :class="{ 'is-active': index === active }"
        :aria-hidden="clone || undefined"
        :inert="clone || undefined"
      >
        <span v-if="card.properties.kicker" class="cinema-card__kicker">{{ card.properties.kicker }}</span>
        <img
          v-if="firstImage(card.properties.icon)"
          class="cinema-card__icon"
          :src="imageUrl(firstImage(card.properties.icon)!, 'square', 480)"
          alt=""
        />
        <h3 class="cinema-card__title">
          <RouterLink v-if="card.properties.link" :to="card.properties.link.route.path">
            {{ card.properties.title }}
          </RouterLink>
          <template v-else>{{ card.properties.title }}</template>
        </h3>
        <p v-if="card.properties.text" class="cinema-card__text">{{ card.properties.text }}</p>
      </article>
    </div>
  </section>
</template>

<style scoped>
/* Card slider. Sits inside the scaled back stack, counter-scaled to stay screen-true. */
.cinema__cards {
  position: absolute;
  left: var(--cards-left);
  right: 0;
  top: var(--cards-top);
  z-index: 2;
  visibility: var(--cards-visibility);
  opacity: var(--cards-opacity);
  filter: blur(var(--cards-blur, 0px));
  transform: translate3d(var(--cards-enter-x), 0, 0) scale(var(--cards-scale));
  transform-origin: 0 0;
  will-change: transform, filter;
}

.cinema__track {
  display: flex;
  gap: clamp(16px, 1.15vw, 24px);
  transform: translate3d(var(--cards-shift), 0, 0);
  transition: transform 640ms cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

.cinema__track.is-jumping {
  transition: none;
}

.cinema-card {
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  flex: 0 0 clamp(320px, 19.4vw, 430px);
  height: 220px;
  padding: 24px;
  overflow: hidden;
  border-radius: 24px;
  color: var(--color-ink);
  background: var(--color-paper);
  box-shadow: 0 18px 52px oklch(20% 0.04 50 / 0.16);
}

.cinema-card__kicker {
  display: block;
  font-family: var(--font-ui);
  font-size: var(--text-label);
  font-weight: 600;
  letter-spacing: var(--tracking-label);
  text-transform: uppercase;
  color: var(--color-muted);
}

.cinema-card__icon {
  position: absolute;
  top: 18px;
  right: 18px;
  width: 72px;
  height: 72px;
  object-fit: contain;
}

.cinema-card__title {
  margin-top: auto;
  max-width: calc(100% - 76px);
  font-size: var(--text-md);
  line-height: 1;
}

.cinema-card__title a {
  color: inherit;
  text-decoration: none;
}

/* The whole card is the link's hit area. */
.cinema-card__title a::after {
  content: '';
  position: absolute;
  inset: 0;
}

.cinema-card:has(a:focus-visible) {
  outline: 3px solid var(--color-focus);
  outline-offset: 3px;
}

.cinema-card__title a:focus-visible {
  outline: none;
}

.cinema-card__text {
  margin: 10px 0 0;
  font-size: var(--text-sm);
  line-height: 1.3;
  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 2;
  overflow: hidden;
}

@media (max-width: 640px) {
  .cinema__track {
    gap: 12px;
  }

  .cinema-card {
    flex-basis: min(82vw, 330px);
  }
}

@media (prefers-reduced-motion: reduce) {
  .cinema__track {
    transition: none;
  }
}
</style>
