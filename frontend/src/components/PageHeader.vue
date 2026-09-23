<script setup lang="ts">
defineProps<{ title: string; intro?: string | null; eyebrow?: string | null }>()
</script>

<template>
  <header class="page-header container">
    <p v-if="eyebrow" class="eyebrow">{{ eyebrow }}</p>
    <h1>{{ title }}</h1>
    <p v-if="intro" class="lead">{{ intro }}</p>
    <slot />
  </header>
</template>

<style scoped>
.page-header {
  padding-block: clamp(2.5rem, 7vw, 5rem) clamp(1.5rem, 4vw, 3rem);
}

.page-header h1 {
  max-width: 18ch;
}
</style>
