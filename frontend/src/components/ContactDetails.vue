<script setup lang="ts">
import { computed } from 'vue'
import type { HomePage } from '../api/types'

// Address and contact channels from the site settings on the root node.
const props = defineProps<{ settings: HomePage['properties'] }>()
const phoneHref = computed(() => `tel:${(props.settings.phone ?? '').replace(/[^\d+]/g, '')}`)
</script>

<template>
  <div class="contact-details">
    <address v-if="settings.address" class="contact-details__address">{{ settings.address }}</address>
    <ul class="contact-details__channels">
      <li v-if="settings.mapUrl"><a :href="settings.mapUrl">Open in a map</a></li>
      <li v-if="settings.phone"><a :href="phoneHref">{{ settings.phone }}</a></li>
      <li v-if="settings.email"><a :href="`mailto:${settings.email}`">{{ settings.email }}</a></li>
    </ul>
  </div>
</template>

<style scoped>
.contact-details__address {
  font-style: normal;
  white-space: pre-line;
  font-family: var(--font-display);
  font-size: var(--text-md);
  font-weight: 400;
  line-height: 1.15;
}

.contact-details__channels {
  margin: var(--space-md) 0 0;
  padding: 0;
  list-style: none;
}

.contact-details__channels li + li {
  margin-top: var(--space-3xs);
}
</style>
