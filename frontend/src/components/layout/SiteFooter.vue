<script setup lang="ts">
import { computed } from 'vue'
import { useSite } from '../../composables/useSite'
import ContactDetails from '../ContactDetails.vue'
import OpeningHours from '../OpeningHours.vue'

const { settings, pageType } = useSite()
// The contact page shows the address and opening hours itself.
const showDetails = computed(() => pageType.value !== 'contactPage')
</script>

<template>
  <footer v-if="settings" class="site-footer page-grid">
    <section v-if="showDetails" class="site-footer__hours" aria-labelledby="footer-hours">
      <h2 id="footer-hours" class="label">Öppettider · Opening hours</h2>
      <OpeningHours :hours="settings.openingHours" />
    </section>

    <section v-if="showDetails" class="site-footer__visit" aria-labelledby="footer-visit">
      <h2 id="footer-visit" class="label">Find us</h2>
      <ContactDetails :settings="settings" />
    </section>

    <div class="site-footer__mast">
      <p class="site-footer__wordmark" aria-hidden="true">{{ settings.logoText ?? settings.siteName }}</p>
      <div class="site-footer__small">
        <p v-if="settings.footerNote">{{ settings.footerNote }}</p>
        <p>
          Site by <a href="https://portfolio-rick.vercel.app/">Lindforge Digital Studio</a>. Photos from Unsplash, credited in
          the README.
        </p>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.site-footer {
  margin-top: var(--space-3xl);
  padding-top: var(--space-2xl);
  border-top: var(--rule-ink);
  row-gap: var(--space-xl);
}

.site-footer .label {
  margin-bottom: var(--space-sm);
}

/* The hours are the reason most people scroll down here, so they get the big type. */
.site-footer__hours :deep(.hours) {
  font-size: var(--text-md);
}

.site-footer__hours :deep(dd) {
  font-size: var(--text-lg);
}

.site-footer__mast {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  justify-content: space-between;
  gap: var(--space-md) var(--space-xl);
  padding-block: var(--space-lg);
  border-top: var(--rule);
}

.site-footer__wordmark {
  margin: 0;
  font-family: var(--font-display);
  font-size: clamp(3.5rem, 12vw, 10rem);
  font-weight: 400;
  letter-spacing: -0.05em;
  line-height: 0.8;
  color: var(--color-ink);
}

.site-footer__small {
  max-width: 44ch;
  font-size: var(--text-sm);
  color: var(--color-muted);
}

.site-footer__small p {
  margin: 0 0 var(--space-2xs);
}

@media (min-width: 48rem) {
  .site-footer__hours {
    grid-column: 2 / 9;
  }

  .site-footer__visit {
    grid-column: 10 / 14;
  }
}
</style>
