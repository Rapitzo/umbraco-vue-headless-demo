<script setup lang="ts">
import SiteFooter from './components/layout/SiteFooter.vue'
import SiteHeader from './components/layout/SiteHeader.vue'
</script>

<template>
  <a href="#main" class="skip-link">Skip to content</a>
  <SiteHeader />
  <main id="main" tabindex="-1">
    <RouterView />
  </main>
  <SiteFooter />
</template>

<style scoped>
.skip-link {
  position: absolute;
  left: 1rem;
  top: -4rem;
  z-index: 10;
  padding: 0.6rem 1rem;
  background: var(--ink);
  color: var(--surface);
  border-radius: var(--radius);
}

.skip-link:focus {
  top: 1rem;
}

main:focus {
  outline: none;
}
</style>
