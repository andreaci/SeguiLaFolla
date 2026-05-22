<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const username = ref('')
const password = ref('')

async function submit() {
  await auth.login(username.value, password.value)
  const redirect = (route.query.redirect as string) || '/'
  router.push(redirect)
}
</script>

<template>
  <section class="app-main--narrow" style="margin: 0 auto">
    <h1>Accedi</h1>
    <BaseAlert v-if="auth.error">{{ auth.error }}</BaseAlert>
    <form @submit.prevent="submit">
      <div class="field">
        <label>Username</label>
        <input v-model="username" class="input" required autocomplete="username" />
      </div>
      <div class="field">
        <label>Password</label>
        <input v-model="password" class="input" type="password" required autocomplete="current-password" />
      </div>
      <BaseButton type="submit" block :disabled="auth.loading">Entra</BaseButton>
    </form>
    <p class="text-muted text-center">
      <router-link to="/registrati">Crea account</router-link>
      ·
      <router-link to="/">Home</router-link>
    </p>
  </section>
</template>
