<script setup lang="ts">
import { computed } from 'vue'
import { firstImage } from '../api/media'
import type { ContactPage } from '../api/types'
import ContactDetails from '../components/ContactDetails.vue'
import ContactForm from '../components/ContactForm.vue'
import PageHeader from '../components/PageHeader.vue'
import ResponsiveImage from '../components/ResponsiveImage.vue'
import { useSite } from '../composables/useSite'

const props = defineProps<{ page: ContactPage }>()

// Contact details are edited once, in the site settings, and shown here and in the footer.
const { settings } = useSite()
const image = computed(() => firstImage(props.page.properties.image))
</script>

<template>
  <PageHeader :title="page.name" :intro="page.properties.intro" />
  <div class="contact container">
    <div class="contact__info">
      <ContactDetails v-if="settings" :settings="settings" class="contact__details" />
      <ResponsiveImage
        v-if="image"
        class="contact__image"
        :media="image"
        preset="standard"
        sizes="(min-width: 64rem) 600px, 100vw"
      />
    </div>
    <ContactForm :intro="page.properties.formIntro" />
  </div>
</template>

<style scoped>
.contact {
  display: grid;
  gap: 3rem;
  align-items: start;
}

.contact__details {
  grid-template-columns: repeat(auto-fit, minmax(12rem, 1fr));
  margin-bottom: 2.5rem;
}

.contact__image {
  width: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
  border-radius: var(--radius);
}

@media (min-width: 64rem) {
  .contact {
    grid-template-columns: 1fr 1fr;
    gap: 4rem;
  }
}
</style>
