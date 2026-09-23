<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useSite } from '../../composables/useSite'

const { settings, navigation } = useSite()
const route = useRoute()
const menuOpen = ref(false)

watch(() => route.path, () => (menuOpen.value = false))

// All pages share one catch-all route, so "active" is worked out from the CMS path:
// a news item keeps "News" highlighted.
const isCurrent = (path: string) => route.path === path || route.path === path.replace(/\/$/, '')
const isSection = (path: string) => route.path.startsWith(path)
</script>

<template>
  <header class="site-header">
    <div class="site-header__inner container">
      <RouterLink to="/" class="logo">
        {{ settings?.logoText ?? settings?.siteName ?? '' }}
        <span v-if="settings?.tagline" class="logo__tagline">{{ settings.tagline }}</span>
      </RouterLink>

      <button
        type="button"
        class="menu-toggle"
        :aria-expanded="menuOpen"
        aria-controls="main-nav"
        @click="menuOpen = !menuOpen"
      >
        {{ menuOpen ? 'Close' : 'Menu' }}
      </button>

      <nav id="main-nav" class="nav" :class="{ 'nav--open': menuOpen }" aria-label="Main">
        <ul>
          <li v-for="item in navigation" :key="item.id">
            <RouterLink
              :to="item.route.path"
              class="nav__link"
              :class="{ 'nav__link--active': isSection(item.route.path) }"
              :aria-current="isCurrent(item.route.path) ? 'page' : undefined"
            >
              {{ item.name }}
            </RouterLink>
          </li>
        </ul>
      </nav>
    </div>
  </header>
</template>

<style scoped>
.site-header {
  border-bottom: 1px solid var(--line);
  background: var(--paper);
}

.site-header__inner {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem 2rem;
  min-height: 4.5rem;
}

.logo {
  display: flex;
  align-items: baseline;
  gap: 0.75rem;
  font-family: var(--font-serif);
  font-size: 1.625rem;
  font-weight: 600;
  color: var(--ink);
  text-decoration: none;
}

.logo__tagline {
  font-family: var(--font-sans);
  font-size: 0.875rem;
  font-weight: 400;
  color: var(--ink-muted);
}

.menu-toggle {
  padding: 0.5rem 0.9rem;
  border: 1px solid var(--line);
  border-radius: var(--radius);
  background: transparent;
  color: var(--ink);
  font: 600 0.9375rem var(--font-sans);
  cursor: pointer;
}

.nav {
  display: none;
  flex-basis: 100%;
}

.nav--open {
  display: block;
}

.nav ul {
  display: flex;
  flex-direction: column;
  margin: 0;
  padding: 0 0 1rem;
  list-style: none;
}

.nav__link {
  display: block;
  padding: 0.75rem 0;
  border-top: 1px solid var(--crumb);
  color: var(--ink);
  font-weight: 500;
  text-decoration: none;
}

.nav__link:hover,
.nav__link--active {
  color: var(--rye);
}

@media (max-width: 30rem) {
  .logo__tagline {
    display: none;
  }
}

@media (min-width: 48rem) {
  .menu-toggle {
    display: none;
  }

  .nav {
    display: block;
    flex-basis: auto;
  }

  .nav ul {
    flex-direction: row;
    gap: 2rem;
    padding: 0;
  }

  .nav__link {
    padding: 0.25rem 0;
    border-top: 0;
    border-bottom: 2px solid transparent;
  }

  .nav__link--active {
    border-bottom-color: var(--rye);
  }
}
</style>
