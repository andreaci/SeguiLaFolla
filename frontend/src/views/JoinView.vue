<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { api } from '../api/client'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const gameId = route.params.id as string
const guestName = ref('')
const error = ref<string | null>(null)
const joining = ref(false)

async function ensureAuth() {
  if (!auth.isLoggedIn) return
  try {
    await api.joinGame(gameId)
    router.replace(`/partita/${gameId}`)
  } catch (e) {
    error.value = (e as Error).message
  }
}

async function joinAsGuest() {
  if (!guestName.value.trim()) return
  joining.value = true
  error.value = null
  try {
    await auth.guest(guestName.value.trim())
    await api.joinGame(gameId)
    router.replace(`/partita/${gameId}`)
  } catch (e) {
    error.value = (e as Error).message
  } finally {
    joining.value = false
  }
}

onMounted(ensureAuth)
</script>

<template>
  <section class="app-main--narrow" style="margin: 0 auto">
    <h1>Entra in partita</h1>
    <p class="text-muted">Partita: {{ gameId }}</p>
    <BaseAlert v-if="error">{{ error }}</BaseAlert>

    <div v-if="!auth.isLoggedIn" class="card">
      <p>Scegli un nome per giocare come ospite:</p>
      <div class="field">
        <label>Nome</label>
        <input v-model="guestName" class="input" maxlength="40" placeholder="Il tuo nome" />
      </div>
      <BaseButton block :disabled="joining" @click="joinAsGuest">Entra come ospite</BaseButton>
      <p class="text-muted text-center" style="margin-top: 1rem">
        <router-link :to="{ name: 'login', query: { redirect: `/entra/${gameId}` } }">
          Accedi con account
        </router-link>
      </p>
    </div>
    <p v-else class="text-muted">Accesso in corso…</p>
  </section>
</template>
