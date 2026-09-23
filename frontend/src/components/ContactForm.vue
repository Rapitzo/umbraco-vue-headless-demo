<script setup lang="ts">
import { reactive, ref } from 'vue'

defineProps<{ intro?: string | null }>()

// Demo only: the form validates and confirms, but nothing leaves the browser.
const form = reactive({ name: '', email: '', message: '' })
const submitted = ref(false)

function onSubmit() {
  submitted.value = true
  form.name = ''
  form.email = ''
  form.message = ''
}
</script>

<template>
  <form class="contact-form" @submit.prevent="onSubmit">
    <h2>Send us a note</h2>
    <p v-if="intro">{{ intro }}</p>
    <p class="contact-form__demo">Demo form: nothing you type here is sent or stored.</p>

    <label for="contact-name">Name</label>
    <input id="contact-name" v-model="form.name" name="name" autocomplete="name" required />

    <label for="contact-email">Email</label>
    <input id="contact-email" v-model="form.email" name="email" type="email" autocomplete="email" required />

    <label for="contact-message">What would you like to order, and for when?</label>
    <textarea id="contact-message" v-model="form.message" name="message" rows="5" required />

    <button type="submit" class="contact-form__send">Send note</button>
    <p class="contact-form__status" role="status">
      <template v-if="submitted">Thanks. This is a demo site, so the message was not sent anywhere.</template>
    </p>
  </form>
</template>

<style scoped>
.contact-form {
  display: grid;
  gap: var(--space-2xs);
  padding-top: var(--space-md);
  border-top: var(--rule-ink);
}

.contact-form h2 {
  font-size: var(--text-lg);
  margin-bottom: var(--space-xs);
}

.contact-form p {
  margin: 0 0 var(--space-xs);
}

.contact-form__demo {
  font-size: var(--text-sm);
  font-style: italic;
  color: var(--color-muted);
}

label {
  margin-top: var(--space-sm);
  font-family: var(--font-display);
  font-size: var(--text-label);
  font-weight: 600;
  letter-spacing: var(--tracking-label);
  text-transform: uppercase;
}

/* Fields are ruled lines, like an order slip. */
input,
textarea {
  width: 100%;
  padding: var(--space-2xs) 0;
  border: 0;
  border-bottom: var(--rule-ink);
  border-radius: 0;
  background: transparent;
  color: var(--color-ink);
  font: inherit;
}

textarea {
  resize: vertical;
  background-image: linear-gradient(transparent calc(100% - 1px), var(--color-rule) 1px);
  background-size: 100% 1.55em;
  line-height: 1.55em;
}

input:hover,
textarea:hover {
  border-bottom-color: var(--color-accent);
}

input:focus-visible,
textarea:focus-visible {
  outline: 3px solid var(--color-focus);
  outline-offset: 2px;
}

input:user-invalid,
textarea:user-invalid {
  border-bottom-color: var(--color-accent-strong);
  border-bottom-style: dashed;
}

.contact-form__send {
  justify-self: start;
  margin-top: var(--space-lg);
  padding: var(--space-xs) var(--space-lg);
  border: var(--rule-ink);
  border-radius: 0;
  background: var(--color-ink);
  color: var(--color-paper);
  font-family: var(--font-display);
  font-size: var(--text-md);
  font-weight: 700;
  font-stretch: 85%;
  cursor: pointer;
  transition: background-color var(--dur-short) var(--ease-out), color var(--dur-short) var(--ease-out);
}

.contact-form__send:hover {
  background: var(--color-accent-strong);
  border-color: var(--color-accent-strong);
}

.contact-form__send:active {
  transform: translateY(1px);
}

.contact-form__send:disabled,
input:disabled,
textarea:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.contact-form__status:empty {
  display: none;
}
</style>
