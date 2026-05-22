<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { api } from '../api/client'
import { hideAuthButtons } from '../config/features'
import { isInAnotherGameError } from '../utils/gameErrors'
import { isDisplayNameTakenInGame, normalizePlayerName } from '../utils/playerName'
import type { PlayerState } from '../types'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import SwitchGamePrompt from '../components/game/SwitchGamePrompt.vue'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const gameId = route.params.id as string
const guestName = ref('')
const error = ref<string | null>(null)
const joining = ref(false)
const players = ref<PlayerState[]>([])
const loadingPlayers = ref(true)

const trimmedName = computed(() => normalizePlayerName(guestName.value))

const showAnotherGamePrompt = computed(() => isInAnotherGameError(error.value))

const nameTaken = computed(() =>
  trimmedName.value.length > 0 &&
  isDisplayNameTakenInGame(players.value, trimmedName.value, auth.user?.id)
)

const canJoinAsGuest = computed(
  () => trimmedName.value.length > 0 && !nameTaken.value && !joining.value
)

async function loadPlayers() {
  loadingPlayers.value = true
  try {
    const state = await api.getGame(gameId)
    players.value = state.players
  } catch (e) {
    error.value = (e as Error).message
  } finally {
    loadingPlayers.value = false
  }
}

async function enterGame() {
  await api.joinGame(gameId)
  await auth.loadMe()
  await router.replace(`/partita/${gameId}`)
}

async function returnToCurrentGame() {
  const id = auth.user?.currentGameId
  if (!id) return
  joining.value = true
  error.value = null
  try {
    const state = await api.getGame(id)
    await router.replace(state.isDirector ? `/direttore/${id}` : `/partita/${id}`)
  } catch (e) {
    error.value = (e as Error).message
    await auth.loadMe()
  } finally {
    joining.value = false
  }
}

async function leaveAndEnterThisGame() {
  const previousId = auth.user?.currentGameId
  if (!previousId) {
    joining.value = true
    error.value = null
    try {
      await enterGame()
    } catch (e) {
      error.value = (e as Error).message
      await loadPlayers()
    } finally {
      joining.value = false
    }
    return
  }

  joining.value = true
  error.value = null
  try {
    await api.leaveGame(previousId)
    await auth.loadMe()
    await enterGame()
  } catch (e) {
    error.value = (e as Error).message
    await loadPlayers()
  } finally {
    joining.value = false
  }
}

async function ensureAuth() {
  if (!auth.isLoggedIn) return
  joining.value = true
  error.value = null
  try {
    if (auth.user?.currentGameId === gameId) {
      await router.replace(`/partita/${gameId}`)
      return
    }
    await enterGame()
  } catch (e) {
    error.value = (e as Error).message
    await loadPlayers()
  } finally {
    joining.value = false
  }
}

async function joinAsGuest() {
  if (!canJoinAsGuest.value) return
  joining.value = true
  error.value = null
  try {
    await auth.guest(trimmedName.value)
    await enterGame()
  } catch (e) {
    error.value = (e as Error).message
    await loadPlayers()
  } finally {
    joining.value = false
  }
}

onMounted(async () => {
  await loadPlayers()
  await ensureAuth()
})
</script>

<template>
  <section class="app-main--narrow" style="margin: 0 auto">
    <h1>Entra in partita</h1>
    <p class="text-muted">Partita: {{ gameId }}</p>

    <SwitchGamePrompt
      v-if="showAnotherGamePrompt"
      :message="error!"
      :loading="joining"
      @switch="leaveAndEnterThisGame"
      @resume="returnToCurrentGame"
    />
    <BaseAlert v-else-if="error">{{ error }}</BaseAlert>



    <div v-else-if="!auth.isLoggedIn" class="card">
      <p>Scegli un nome per giocare come ospite:</p>
      <div class="field">
        <label for="guest-name">Nome</label>
        <input
          id="guest-name"
          v-model="guestName"
          class="input"
          :class="{ 'input--invalid': nameTaken }"
          maxlength="40"
          placeholder="Il tuo nome"
          autocomplete="nickname"
          :disabled="joining || loadingPlayers"
          @keydown.enter.prevent="joinAsGuest"
        />
        <p v-if="nameTaken" class="join__name-error" role="alert">
          Questo nome è già usato in partita. Scegline un altro.
        </p>
      </div>
      <BaseButton block :disabled="!canJoinAsGuest" @click="joinAsGuest">
        Entra come ospite
      </BaseButton>
      <p
        v-if="!hideAuthButtons"
        class="text-muted text-center"
        style="margin-top: 1rem"
      >
        <router-link :to="{ name: 'login', query: { redirect: `/entra/${gameId}` } }">
          Accedi con account
        </router-link>
      </p>
    </div>
    <p v-else-if="joining" class="text-muted">Accesso in corso…</p>
  </section>
</template>

<style scoped>
.join__name-error {
  margin: var(--space-sm) 0 0;
  font-size: 0.9rem;
  font-weight: 700;
  color: var(--color-accent);
}

.join__hint {
  margin-top: var(--space-sm);
  font-size: 0.9rem;
  font-weight: 600;
  text-align: center;
}

.input--invalid {
  border-color: var(--color-accent);
  background: var(--color-accent-soft);
}
</style>
