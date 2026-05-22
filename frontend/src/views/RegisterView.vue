<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'

const auth = useAuthStore()
const router = useRouter()

const username = ref('')
const password = ref('')
const displayName = ref('')

async function submit() {
  await auth.register(username.value, password.value, displayName.value || username.value)
  router.push('/direttore')
}
</script>

<template>
  <section class="app-main--narrow" style="margin: 0 auto">
    <h1>Registrati</h1>
    <BaseAlert v-if="auth.error">{{ auth.error }}</BaseAlert>
    <form @submit.prevent="submit">
      <div class="field">
        <label>Nome in gioco</label>
        <input v-model="displayName" class="input" placeholder="Opzionale" />
      </div>
      <div class="field">
        <label>Username</label>
        <input v-model="username" class="input" required />
      </div>
      <div class="field">
        <label>Password</label>
        <input v-model="password" class="input" type="password" required />
      </div>
      <BaseButton type="submit" block :disabled="auth.loading">Crea account</BaseButton>
    </form>
    <p class="text-muted text-center">
      <router-link to="/accedi">Hai già un account?</router-link>
    </p>
  </section>
</template>
