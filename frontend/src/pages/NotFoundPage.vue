<script setup lang="ts">
import PageHeader from '../components/PageHeader.vue'
</script>

<template>
  <PageHeader title="Page not found" intro="The page may have moved, or the link is out of date.">
    <RouterLink to="/" class="button">Go to the front page</RouterLink>
  </PageHeader>
</template>
