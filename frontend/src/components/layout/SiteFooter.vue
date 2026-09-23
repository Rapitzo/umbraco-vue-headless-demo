<script setup lang="ts">
import { useSite } from '../../composables/useSite'
import ContactDetails from '../ContactDetails.vue'

const { settings } = useSite()
</script>

<template>
  <footer v-if="settings" class="site-footer">
    <div class="site-footer__grid container">
      <div>
        <p class="site-footer__name">{{ settings.siteName }}</p>
        <p v-if="settings.tagline">{{ settings.tagline }}</p>
      </div>
      <ContactDetails :settings="settings" />
    </div>

    <div class="site-footer__meta container">
      <p v-if="settings.footerNote">{{ settings.footerNote }}</p>
      <p>
        Demo site by <a href="https://portfolio-rick.vercel.app/">Lindforge Digital Studio</a>. Photos from Unsplash,
        credited in the README.
      </p>
    </div>
  </footer>
</template>

<style scoped>
.site-footer {
  margin-top: var(--space-section);
  padding-top: clamp(2.5rem, 6vw, 4rem);
  background: var(--ink);
  color: var(--crumb);
  font-size: 0.9375rem;
}

.site-footer :deep(a) {
  color: var(--surface);
}

.site-footer__grid {
  display: grid;
  gap: 2rem;
}

.site-footer__name {
  margin-bottom: 0.25rem;
  font-family: var(--font-serif);
  font-size: 1.5rem;
  color: var(--surface);
}

.site-footer__meta {
  margin-top: 2.5rem;
  padding-block: 1.5rem;
  border-top: 1px solid #4a3f35;
  color: #c9b9a3;
  font-size: 0.875rem;
}

.site-footer__meta p {
  margin: 0 0 0.25rem;
}

@media (min-width: 64rem) {
  .site-footer__grid {
    grid-template-columns: 1fr 3fr;
  }
}
</style>
