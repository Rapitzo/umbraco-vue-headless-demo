<script setup lang="ts">
import { computed } from 'vue'
import type { HomePage } from '../api/types'
import { parseOpeningHours } from '../utils/openingHours'

// Address, opening hours and contact channels from the site settings on the root node.
// Used by the footer and the contact page; colours are inherited from the parent.
const props = withDefaults(defineProps<{ settings: HomePage['properties']; headingTag?: 'h2' | 'h3' }>(), {
  headingTag: 'h2',
})

const hours = computed(() => parseOpeningHours(props.settings.openingHours))
const phoneHref = computed(() => `tel:${(props.settings.phone ?? '').replace(/[^\d+]/g, '')}`)
</script>

<template>
  <div class="contact-details">
    <div v-if="settings.address" class="contact-details__group">
      <component :is="headingTag" class="contact-details__heading">Find us</component>
      <address>{{ settings.address }}</address>
    </div>

    <div v-if="hours.length" class="contact-details__group">
      <component :is="headingTag" class="contact-details__heading">Opening hours</component>
      <dl class="contact-details__hours">
        <template v-for="row in hours" :key="row.days">
          <dt>{{ row.days }}</dt>
          <dd>{{ row.hours }}</dd>
        </template>
      </dl>
    </div>

    <div v-if="settings.phone || settings.email" class="contact-details__group">
      <component :is="headingTag" class="contact-details__heading">Contact</component>
      <ul class="contact-details__channels">
        <li v-if="settings.phone"><a :href="phoneHref">{{ settings.phone }}</a></li>
        <li v-if="settings.email"><a :href="`mailto:${settings.email}`">{{ settings.email }}</a></li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.contact-details {
  display: grid;
  gap: 2rem;
  grid-template-columns: repeat(auto-fit, minmax(13rem, 1fr));
}

.contact-details__heading {
  margin-bottom: 0.75rem;
  font-family: var(--font-sans);
  font-size: 0.8125rem;
  font-weight: 600;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  opacity: 0.8;
}

address {
  font-style: normal;
  white-space: pre-line;
}

.contact-details__hours {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 0.25rem 1rem;
  margin: 0;
}

.contact-details__hours dd {
  margin: 0;
}

.contact-details__channels {
  margin: 0;
  padding: 0;
  list-style: none;
}

.contact-details__channels li + li {
  margin-top: 0.25rem;
}
</style>
