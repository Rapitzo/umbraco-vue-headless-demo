<script setup lang="ts">
import PageHeader from '../components/PageHeader.vue'
</script>

<template>
  <PageHeader title="Page not found" intro="The page may have moved, or the link is out of date.">
    <p><RouterLink to="/" class="link-cta">Back to the front page</RouterLink></p>
  </PageHeader>
</template>
