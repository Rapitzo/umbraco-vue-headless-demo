<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../api/media'
import type { ContactPage } from '../api/types'
import ContactDetails from '../components/ContactDetails.vue'
import ContactForm from '../components/ContactForm.vue'
import OpeningHours from '../components/OpeningHours.vue'
import PageHeader from '../components/PageHeader.vue'
import ResponsiveImage from '../components/ResponsiveImage.vue'
import { useSite } from '../composables/useSite'

const props = defineProps<{ page: ContactPage }>()

// Address and hours are edited once, in the site settings, and shown here and in the footer.
const { settings } = useSite()
const image = computed(() => firstImage(props.page.properties.image))
</script>

<template>
  <PageHeader :title="page.name" :intro="page.properties.intro" />
  <div class="visit page-grid">
    <ResponsiveImage
      v-if="image"
      class="visit__image"
      :media="image"
      preset="standard"
      sizes="(min-width: 48rem) 50vw, 100vw"
    />
    <section v-if="settings" class="visit__details" aria-labelledby="visit-where">
      <h2 id="visit-where" class="label">Address</h2>
      <ContactDetails :settings="settings" />
      <h2 class="label visit__hours-label">Öppettider · Opening hours</h2>
      <OpeningHours :hours="settings.openingHours" />
    </section>
    <ContactForm class="visit__form" :intro="page.properties.formIntro" />
  </div>
</template>

<style scoped>
.visit {
  row-gap: var(--space-2xl);
}

.visit__image {
  grid-column: full;
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

.visit .label {
  margin-bottom: var(--space-sm);
}

.visit__hours-label {
  margin-top: var(--space-xl);
}

@media (min-width: 48rem) {
  .visit__image {
    grid-column: full-start / 8;
    grid-row: 1 / 3;
    position: sticky;
    top: var(--space-lg);
  }

  .visit__details {
    grid-column: 9 / 14;
  }

  .visit__form {
    grid-column: 9 / 14;
  }
}
</style>
