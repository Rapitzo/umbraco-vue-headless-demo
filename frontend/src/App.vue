<template>
  <div class="shell">
    <nav class="top">
      <RouterLink to="/" class="brand">Umbraco + Vue</RouterLink>
      <span class="tag">Content Delivery API demo</span>
    </nav>
    <main>
      <RouterView />
    </main>
    <footer>Rickard Lindbom · Lindforge Digital Studio</footer>
  </div>
</template>
