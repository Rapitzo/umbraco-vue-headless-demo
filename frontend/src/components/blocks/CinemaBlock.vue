<script setup lang="ts">
import { computed, ref } from 'vue'
import { firstImage, imageSrcset, imageUrl } from '../../api/media'
import type { CinemaBlock, MediaItem } from '../../api/types'
import { useCinemaScroll } from '../../composables/useCinemaScroll'
import CinemaBoard from './CinemaBoard.vue'
import CinemaCards from './CinemaCards.vue'
import CinemaCopy from './CinemaCopy.vue'

const props = defineProps<{ block: CinemaBlock }>()
const p = computed(() => props.block.properties)

const section = ref<HTMLElement | null>(null)
const stage = ref<HTMLElement | null>(null)
const bakes = computed(() => p.value.bakes?.items.map((item) => item.content) ?? [])
// The timeline length is fixed at mount, so whether the board scene exists is read once.
const { controlsReady } = useCinemaScroll(section, stage, bakes.value.length > 0)

// Scene layers are decorative, so they render with empty alt text whatever the media says.
function layer(images: MediaItem[] | null) {
  const media = firstImage(images)
  return media && { src: imageUrl(media, 'original', 1600), srcset: imageSrcset(media, 'original') }
}
const sky = computed(() => layer(p.value.skyLayer))
const back = computed(() => layer(p.value.backLayer))
const foreground = computed(() => layer(p.value.foregroundLayer))
const splitLeft = computed(() => layer(p.value.splitLeftLayer))
const closeUp = computed(() => layer(p.value.closeUpLayer))

const cards = computed(() => p.value.cards?.items.map((item) => item.content) ?? [])
const slider = ref<InstanceType<typeof CinemaCards> | null>(null)
</script>

<template>
  <section ref="section" class="cinema" :class="{ 'cinema--board': bakes.length }" :aria-label="block.properties.heading">
    <div ref="stage" class="cinema__stage">
      <div class="cinema__world">
        <img v-if="sky" class="cinema__layer cinema__sky" v-bind="sky" sizes="100vw" alt="" />

        <div class="cinema__back-stack">
          <CinemaCards v-if="cards.length" ref="slider" :cards="cards" />
          <img v-if="back" class="cinema__layer cinema__back" v-bind="back" sizes="120vw" alt="" />
        </div>

        <div v-if="cards.length > 1" class="cinema__controls" :class="{ 'is-ready': controlsReady }">
          <button type="button" class="cinema__nav" aria-label="Previous" @click="slider?.move(-1)">←</button>
          <button type="button" class="cinema__nav" aria-label="Next" @click="slider?.move(1)">→</button>
        </div>

        <h2 class="cinema__title">{{ block.properties.heading }}</h2>

        <img v-if="splitLeft" class="cinema__layer cinema__split cinema__split--left" v-bind="splitLeft" sizes="120vw" alt="" />
        <img v-if="splitLeft" class="cinema__layer cinema__split cinema__split--right" v-bind="splitLeft" sizes="120vw" alt="" />
        <img v-if="foreground" class="cinema__layer cinema__foreground" v-bind="foreground" sizes="(max-width: 40rem) 190vw, 70vw" alt="" />
        <img v-if="closeUp" class="cinema__layer cinema__close-up" v-bind="closeUp" sizes="125vw" alt="" />
        <div class="cinema__shade" />
      </div>
      <CinemaCopy :block="block" />
      <CinemaBoard
        v-if="bakes.length"
        :heading="block.properties.boardHeading"
        :intro="block.properties.boardIntro"
        :bakes="bakes"
      />
      <div class="cinema__outro" />
    </div>
  </section>
</template>

<style scoped>
/* Scroll rig: a tall section with a sticky, viewport-high stage. useCinemaScroll writes the
   custom properties below from the scroll position; the defaults are the resting state. */
.cinema {
  --back-opacity: 1;
  --back-x: 0px;
  --back-y: 0px;
  --back-scale: 0.76;
  --back-layer-y: 20vh;
  --blur-px: 0px;
  --back-brightness: 1;
  --back-layer-blur-px: 0px;
  --back-layer-brightness: 1;
  --back-layer-saturation: 1;
  --shade-z: 0;
  --shade-top: 0;
  --shade-mid: 0;
  --shade-bottom: 0;
  --title-y: 0px;
  --title-scale: 1;
  --title-opacity: 1;
  --fg-x: -50%;
  --fg-y: 0px;
  --fg-bottom: 5vh;
  --fg-width: 52vw;
  --fg-scale: 1.02;
  --fg-opacity: 1;
  --split-left-x: 0px;
  --split-right-x: 0px;
  --split-y: 0px;
  --split-scale: 1;
  --split-opacity: 1;
  --close-opacity: 0;
  --close-x: -50%;
  --close-y: -50%;
  --close-scale: 1.06;
  --intro-y: 0px;
  --intro-opacity: 1;
  --panel1-opacity: 0;
  --panel1-y: calc(-50% + 58px);
  --panel2-opacity: 0;
  --panel2-y: calc(-50% + 58px);
  --cards-visibility: hidden;
  --cards-enter-x: 140vw;
  --cards-opacity: 1;
  --cards-scale: 1;
  --cards-top: 0px;
  --cards-left: 48px;
  --cards-screen-top: clamp(62px, 19vh - 50px, 170px);
  --controls-opacity: 0;
  --controls-visibility: hidden;
  --bakes-progress: 0;
  --board-opacity: 0;
  --board-y: 80px;
  --board-visibility: hidden;
  --outro: 0;
  --panel2-visibility: hidden;

  /* Warm rye tint for the blur wash between scenes. */
  --tint: 120, 74, 38;
  --cinema-paper: var(--color-on-image);
  --text-shadow: 0 2px 18px oklch(15% 0.02 50 / 0.45);

  position: relative;
  height: calc(100vh + 3100px);
  color: var(--cinema-paper);
}

.cinema--board {
  height: calc(100vh + 3950px);
}

.cinema__stage {
  position: sticky;
  top: 0;
  height: 100vh;
  min-height: 620px;
  overflow: hidden;
  isolation: isolate;
  background: linear-gradient(180deg, oklch(80% 0.06 75), oklch(64% 0.08 60));
}

.cinema__world,
.cinema__back-stack,
.cinema__layer,
.cinema__shade,
.cinema__controls,
.cinema__title {
  position: absolute;
}

.cinema__world {
  inset: 0;
  overflow: hidden;
}

.cinema__layer {
  display: block;
  max-width: none;
  user-select: none;
  pointer-events: none;
  will-change: transform, opacity, filter;
}

.cinema__sky {
  inset: 0;
  z-index: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: blur(var(--blur-px)) brightness(var(--back-brightness));
}

.cinema__back-stack {
  inset: 0 -3vw;
  z-index: 1;
  opacity: var(--back-opacity);
  transform: translate3d(var(--back-x), var(--back-y), 0) scale(var(--back-scale));
  transform-origin: 50% 100%;
  will-change: transform, opacity;
}

.cinema__back {
  bottom: 0;
  left: 48%;
  z-index: 3;
  width: 112%;
  height: auto;
  filter: blur(var(--back-layer-blur-px)) brightness(var(--back-layer-brightness))
    saturate(var(--back-layer-saturation));
  transform: translate3d(-50%, var(--back-layer-y), 0) scale(0.86);
}

.cinema__title {
  left: 50%;
  top: clamp(122px, 19vh, 205px);
  z-index: 3;
  width: min(94vw, 1780px);
  margin: 0;
  color: var(--cinema-paper);
  font-size: clamp(3rem, 16.5vw, 16rem);
  line-height: 0.78;
  text-align: center;
  transform: translate3d(-50%, var(--title-y), 0) scale(var(--title-scale));
  opacity: var(--title-opacity);
  will-change: transform, opacity;
}

.cinema__foreground {
  left: 50%;
  bottom: var(--fg-bottom);
  z-index: 4;
  width: min(var(--fg-width), 2140px);
  height: auto;
  opacity: var(--fg-opacity);
  transform: translate3d(var(--fg-x), var(--fg-y), 0) scale(var(--fg-scale));
  transform-origin: 50% 48%;
}

.cinema__close-up {
  left: 50%;
  top: 50%;
  z-index: 5;
  width: min(122vw, 2160px);
  height: auto;
  opacity: var(--close-opacity);
  transform: translate3d(var(--close-x), var(--close-y), 0) scale(var(--close-scale));
  transform-origin: 50% 48%;
}

/* Split frames are full canvases with content on one side only. Sized by height and pinned
   to their own edge, so the content frames the view whatever the viewport's aspect ratio. */
.cinema__split {
  bottom: -2vh;
  z-index: 6;
  width: auto;
  height: min(92vh, 66vw);
  opacity: var(--split-opacity);
}

.cinema__split--left {
  left: 0;
  transform: translate3d(var(--split-left-x), var(--split-y), 0) scale(var(--split-scale));
  transform-origin: 16% 70%;
}

.cinema__split--right {
  right: 0;
  /* The left frame, mirrored: flip about the image centre (translateX + scaleX), then scale
     and drift about the content like the left one. */
  transform: translate3d(var(--split-right-x), var(--split-y), 0) scale(var(--split-scale)) translateX(-68%) scaleX(-1);
  transform-origin: 84% 70%;
}

.cinema__shade {
  inset: 0;
  z-index: var(--shade-z);
  pointer-events: none;
  background: linear-gradient(
    180deg,
    rgba(var(--tint), var(--shade-top)) 0%,
    rgba(var(--tint), var(--shade-mid)) 48%,
    rgba(var(--tint), var(--shade-bottom)) 100%
  );
}

/* Hands over to the page below: the bottom edge of the last scene fades into the page colour
   just before the stage scrolls away, so there is no hard edge. */
.cinema__outro {
  position: absolute;
  inset: auto 0 0;
  z-index: 11;
  height: 22%;
  background: linear-gradient(180deg, transparent, var(--color-paper) 90%);
  opacity: var(--outro);
  pointer-events: none;
}

/* Slider arrows, in screen space under the cards. */
.cinema__controls {
  left: 48px;
  top: calc(var(--cards-screen-top) + 240px + 16px);
  z-index: 5;
  display: flex;
  gap: 14px;
  opacity: var(--controls-opacity);
  visibility: var(--controls-visibility);
  pointer-events: none;
}

.cinema__controls.is-ready {
  pointer-events: auto;
}

.cinema__nav {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 54px;
  height: 54px;
  border: 0;
  border-radius: 999px;
  color: var(--color-ink);
  background: var(--color-paper);
  font: inherit;
  font-size: 1.25rem;
  box-shadow: 0 18px 36px oklch(15% 0.02 50 / 0.2);
  cursor: pointer;
}

.cinema__nav:hover {
  color: var(--color-accent-strong);
}

@media (max-width: 1100px) {
  .cinema__foreground {
    width: 88vw;
  }

  .cinema__close-up {
    width: 132vw;
  }
}

@media (max-width: 640px) {
  .cinema__stage {
    min-height: 640px;
  }

  .cinema__title {
    top: 16vh;
  }

  .cinema__foreground {
    bottom: 2vh;
    width: 118vw;
  }

  /* Portrait screens: fill the height instead, or the scene behind shows through. */
  .cinema__close-up {
    width: auto;
    height: 112vh;
  }
}
</style>
