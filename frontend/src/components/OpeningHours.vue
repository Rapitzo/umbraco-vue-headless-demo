<script setup lang="ts">
import { computed } from 'vue'
import { parseOpeningHours } from '../utils/openingHours'

// Opening hours from the site settings, set as a two-column timetable.
const props = defineProps<{ hours: string | null }>()
const rows = computed(() => parseOpeningHours(props.hours))
</script>

<template>
  <dl v-if="rows.length" class="hours">
    <div v-for="row in rows" :key="row.days" class="hours__row">
      <dt>{{ row.days }}</dt>
      <dd>{{ row.hours }}</dd>
    </div>
  </dl>
</template>

<style scoped>
.hours {
  margin: 0;
  font-variant-numeric: tabular-nums lining-nums;
}

.hours__row {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 0 var(--space-md);
  padding-block: var(--space-xs);
  border-bottom: var(--rule);
}

.hours__row:first-child {
  border-top: var(--rule-ink);
}

dt {
  font-style: italic;
}

dd {
  margin: 0;
  font-family: var(--font-display);
  font-weight: 700;
  font-stretch: 85%;
}
</style>
