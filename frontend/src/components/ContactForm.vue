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

    <button type="submit" class="button">Send</button>
    <p class="contact-form__status" role="status">
      <template v-if="submitted">Thanks. This is a demo site, so the message was not sent anywhere.</template>
    </p>
  </form>
</template>

<style scoped>
.contact-form {
  display: grid;
  gap: 0.5rem;
  padding: clamp(1.5rem, 4vw, 2.5rem);
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: var(--radius);
}

.contact-form h2 {
  margin-bottom: 0.25rem;
}

.contact-form p {
  margin: 0 0 0.5rem;
}

.contact-form__demo {
  padding: 0.6rem 0.8rem;
  background: var(--crumb);
  border-radius: var(--radius);
  font-size: 0.9375rem;
}

label {
  margin-top: 0.5rem;
  font-weight: 600;
}

input,
textarea {
  width: 100%;
  padding: 0.7rem 0.8rem;
  border: 1px solid #8f8274;
  border-radius: var(--radius);
  background: #fff;
  color: var(--ink);
  font: inherit;
}

input:focus-visible,
textarea:focus-visible {
  outline: 3px solid var(--focus);
  outline-offset: 1px;
  border-color: var(--focus);
}

.button {
  justify-self: start;
  margin-top: 1rem;
}

.contact-form__status:empty {
  display: none;
}
</style>
