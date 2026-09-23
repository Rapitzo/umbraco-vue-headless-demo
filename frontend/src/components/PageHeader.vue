<script setup lang="ts">
defineProps<{ title: string; intro?: string | null }>()
</script>

<template>
  <header class="page-header page-grid">
    <div class="page-header__meta"><slot name="meta" /></div>
    <h1 class="page-header__title">{{ title }}</h1>
    <p v-if="intro" class="page-header__intro lead">{{ intro }}</p>
    <slot />
  </header>
</template>

<style scoped>
.page-header {
  padding-block: var(--space-2xl) var(--space-3xl);
  row-gap: var(--space-lg);
}

.page-header__meta:empty {
  display: none;
}

.page-header__title {
  font-size: var(--text-display);
  max-width: 12ch;
}

/* The intro steps in from the left edge so the title owns the corner. */
@media (min-width: 48rem) {
  .page-header__intro {
    grid-column: 6 / 14;
  }
}
</style>
