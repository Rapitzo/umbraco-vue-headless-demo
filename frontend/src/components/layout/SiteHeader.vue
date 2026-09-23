<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useSite } from '../../composables/useSite'
import { hoursForDate, parseOpeningHours } from '../../utils/openingHours'

const { settings, navigation } = useSite()
const route = useRoute()
const menuOpen = ref(false)

watch(() => route.path, () => (menuOpen.value = false))

// The masthead's issue line: today's hours, worked out from the site settings.
const today = computed(() => {
  const hours = hoursForDate(parseOpeningHours(settings.value?.openingHours), new Date())
  if (!hours) return null
  return hours.toLowerCase() === 'closed' ? 'Closed today' : `Open today ${hours}`
})

// All pages share one catch-all route, so "active" is worked out from the CMS path:
// a news item keeps "News" highlighted.
const isCurrent = (path: string) => route.path === path || route.path === path.replace(/\/$/, '')
const isSection = (path: string) => route.path.startsWith(path)
</script>

<template>
  <header class="masthead">
    <div class="masthead__issue">
      <span v-if="settings?.tagline" class="masthead__tagline">{{ settings.tagline }}</span>
      <span v-if="today">{{ today }}</span>
    </div>

    <div class="masthead__main">
      <RouterLink to="/" class="wordmark">{{ settings?.logoText ?? settings?.siteName ?? '' }}</RouterLink>
      <button
        type="button"
        class="menu-toggle label"
        :aria-expanded="menuOpen"
        aria-controls="main-nav"
        @click="menuOpen = !menuOpen"
      >
        {{ menuOpen ? 'Close' : 'Menu' }}
      </button>
    </div>

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
  </header>
</template>

<style scoped>
.masthead {
  padding-inline: var(--gutter);
  border-bottom: var(--rule-ink);
}

.masthead__issue {
  display: flex;
  justify-content: space-between;
  gap: var(--space-md);
  padding-block: var(--space-2xs);
  border-bottom: var(--rule);
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}

.masthead__main {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-block: var(--space-sm);
}

.wordmark {
  font-family: var(--font-display);
  font-size: clamp(2.25rem, 4vw + 1rem, 4.5rem);
  font-weight: 800;
  font-stretch: 75%;
  letter-spacing: -0.04em;
  line-height: 0.9;
  color: var(--color-ink);
  text-decoration: none;
  white-space: nowrap;
}

.wordmark:hover {
  color: var(--color-accent-strong);
}

.menu-toggle {
  padding: var(--space-2xs) var(--space-sm);
  border: var(--rule-ink);
  border-radius: 0;
  background: transparent;
  color: var(--color-ink);
  cursor: pointer;
}

.menu-toggle:hover,
.menu-toggle[aria-expanded='true'] {
  background: var(--color-ink);
  color: var(--color-paper);
}

.menu-toggle:active {
  transform: translateY(1px);
}

.nav {
  display: none;
}

.nav--open {
  display: block;
}

.nav ul {
  margin: 0;
  padding: 0 0 var(--space-sm);
  list-style: none;
}

.nav__link {
  display: block;
  padding-block: var(--space-xs);
  border-top: var(--rule);
  font-family: var(--font-display);
  font-size: var(--text-md);
  font-weight: 700;
  font-stretch: 85%;
  color: var(--color-ink);
  text-decoration: none;
  white-space: nowrap;
}

.nav__link:hover,
.nav__link--active {
  color: var(--color-accent);
}

@media (max-width: 30rem) {
  .masthead__tagline {
    display: none;
  }
}

/* Desktop: a newspaper masthead. Wordmark centred, sections in one row beneath it. */
@media (min-width: 48rem) {
  .masthead__main {
    justify-content: center;
    padding-block: var(--space-md) var(--space-xs);
  }

  .menu-toggle {
    display: none;
  }

  .nav {
    display: block;
  }

  .nav ul {
    display: flex;
    justify-content: center;
    gap: var(--space-xl);
    padding-bottom: var(--space-sm);
  }

  .nav__link {
    padding: var(--space-3xs) 0;
    border-top: 0;
    border-bottom: 2px solid transparent;
    font-family: var(--font-display);
    font-size: var(--text-label);
    font-weight: 600;
    font-stretch: 100%;
    letter-spacing: var(--tracking-label);
    text-transform: uppercase;
  }

  .nav__link--active {
    border-bottom-color: var(--color-accent);
  }
}
</style>
