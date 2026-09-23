<script setup lang="ts">
import { computed } from 'vue'
import type { CinemaBlock } from '../../api/types'

// The text over the cinema scene: intro, two story panels. Positions and fades come from the
// custom properties useCinemaScroll sets on the enclosing .cinema section.
const props = defineProps<{ block: CinemaBlock }>()

const tags = computed(() =>
  (props.block.properties.tags ?? '')
    .split(',')
    .map((tag) => tag.trim())
    .filter(Boolean),
)
const facts = computed(() =>
  [
    [props.block.properties.fact1Value, props.block.properties.fact1Label],
    [props.block.properties.fact2Value, props.block.properties.fact2Label],
  ].filter(([value]) => value),
)
</script>

<template>
  <div v-if="block.properties.intro || tags.length" class="cinema__intro">
    <p v-if="block.properties.intro">{{ block.properties.intro }}</p>
    <ul v-if="tags.length" class="cinema__tags">
      <li v-for="tag in tags" :key="tag">{{ tag }}</li>
    </ul>
  </div>

  <div v-if="block.properties.firstHeading" class="cinema__panel cinema__panel--first">
    <h3>{{ block.properties.firstHeading }}</h3>
    <p v-if="block.properties.firstText">{{ block.properties.firstText }}</p>
    <dl v-if="facts.length" class="cinema__facts">
      <div v-for="[value, label] in facts" :key="value!">
        <dt>{{ value }}</dt>
        <dd>{{ label }}</dd>
      </div>
    </dl>
  </div>

  <div v-if="block.properties.secondHeading" class="cinema__panel cinema__panel--second">
    <h3>{{ block.properties.secondHeading }}</h3>
    <p v-if="block.properties.secondText">{{ block.properties.secondText }}</p>
    <RouterLink
      v-if="block.properties.ctaLink && block.properties.ctaLabel"
      :to="block.properties.ctaLink.route.path"
      class="cinema__pill cinema__cta"
    >
      <span aria-hidden="true">↗</span>
      {{ block.properties.ctaLabel }}
    </RouterLink>
  </div>
</template>

<style scoped>
.cinema__intro,
.cinema__panel {
  position: absolute;
}

.cinema__intro {
  left: 50%;
  bottom: clamp(56px, 22vh, 400px);
  z-index: 9;
  width: min(560px, calc(100vw - 40px));
  text-align: center;
  transform: translate3d(-50%, var(--intro-y), 0);
  opacity: var(--intro-opacity);
  will-change: transform, opacity;
}

/* The intro sits over the busiest part of the scene, so it gets a frosted backing. */
.cinema__intro p {
  padding: 18px 26px;
  border-radius: 24px;
  background: oklch(22% 0.03 50 / 0.5);
  backdrop-filter: blur(14px) saturate(1.1);
  -webkit-backdrop-filter: blur(14px) saturate(1.1);
}

.cinema__intro p,
.cinema__panel p {
  margin: 0 auto;
  max-width: 34rem;
  font-size: var(--text-md);
  line-height: 1.25;
  text-shadow: var(--text-shadow);
}

.cinema__tags {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 10px;
  margin: 26px 0 0;
  padding: 0;
  list-style: none;
}

.cinema__pill,
.cinema__tags li {
  display: inline-flex;
  align-items: center;
  min-height: 42px;
  padding: 0 24px;
  border-radius: 999px;
  color: var(--color-ink);
  background: var(--color-paper);
  font-family: var(--font-ui);
  font-size: var(--text-sm);
  font-weight: 600;
  box-shadow: 0 12px 30px oklch(15% 0.02 50 / 0.18);
}

.cinema__panel {
  left: 50%;
  top: 45%;
  z-index: 10;
  width: min(760px, calc(100vw - 42px));
  text-align: center;
  pointer-events: none;
  will-change: transform, opacity;
}

.cinema__panel h3 {
  font-size: clamp(2.45rem, 5vw + 0.5rem, 5.5rem);
  color: var(--cinema-paper);
  text-shadow: 0 16px 38px oklch(15% 0.02 50 / 0.32);
}

.cinema__panel p {
  margin-top: 26px;
}

.cinema__panel--first {
  top: 60%;
  opacity: var(--panel1-opacity);
  transform: translate3d(-50%, var(--panel1-y), 0);
}

.cinema__panel--second {
  top: 29%;
  opacity: var(--panel2-opacity);
  visibility: var(--panel2-visibility);
  transform: translate3d(-50%, var(--panel2-y), 0);
}

.cinema__facts {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: clamp(18px, 5vw, 86px);
  width: min(470px, 100%);
  margin: clamp(34px, 5vw, 72px) auto 0;
}

.cinema__facts dt {
  font-family: var(--font-display);
  font-size: clamp(2.5rem, 4vw + 0.5rem, 4.2rem);
  font-weight: 400;
  line-height: 0.9;
  letter-spacing: var(--tracking-display);
  text-shadow: 0 14px 34px oklch(15% 0.02 50 / 0.32);
}

.cinema__facts dd {
  margin: 18px 0 0;
  font-size: var(--text-sm);
  line-height: 1.2;
  text-shadow: var(--text-shadow);
}

.cinema__cta {
  gap: 12px;
  min-height: 50px;
  margin-top: 28px;
  text-decoration: none;
  pointer-events: auto;
}

.cinema__cta:hover {
  color: var(--color-accent-strong);
}

@media (max-width: 640px) {
  .cinema__intro {
    bottom: 42px;
  }

  .cinema__intro p,
  .cinema__panel p {
    font-size: var(--text-base);
  }

  .cinema__tags li {
    min-height: 38px;
    padding: 0 16px;
  }

  .cinema__panel {
    top: 42%;
  }

  .cinema__panel--second {
    top: 26%;
  }
}
</style>
