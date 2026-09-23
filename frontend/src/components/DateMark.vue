<script setup lang="ts">
import { computed } from 'vue'

// A date set as type: the day large, month and year small beneath it.
const props = withDefaults(defineProps<{ value: string; size?: 'lg' | 'sm' }>(), { size: 'sm' })

const date = computed(() => new Date(props.value))
const day = computed(() => date.value.getDate())
const monthYear = computed(() =>
  new Intl.DateTimeFormat('en-GB', { month: 'long', year: 'numeric' }).format(date.value),
)
</script>

<template>
  <time class="date-mark" :class="`date-mark--${size}`" :datetime="value">
    <span class="date-mark__day">{{ day }}</span>
    <span class="date-mark__rest label">{{ monthYear }}</span>
  </time>
</template>

<style scoped>
.date-mark {
  display: inline-grid;
  justify-items: start;
  font-variant-numeric: lining-nums tabular-nums;
}

.date-mark__day {
  font-family: var(--font-display);
  font-size: var(--text-xl);
  font-weight: 800;
  font-stretch: 75%;
  letter-spacing: var(--tracking-display);
  line-height: 0.85;
  color: var(--color-accent);
}

.date-mark--lg .date-mark__day {
  font-size: var(--text-display);
}

.date-mark__rest {
  margin-top: var(--space-2xs);
}
</style>
