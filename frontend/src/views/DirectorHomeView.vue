<script setup lang="ts">
import { computed } from 'vue'
import { useDirectorStart } from '../composables/useDirectorStart'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import AlreadyInGamePrompt from '../components/game/AlreadyInGamePrompt.vue'

const {
  auth,
  gameName,
  error,
  loading,
  hasCurrentGame,
  isAlreadyInGameError,
  startAsGuestDirector,
  createGameWithCurrentUser,
  resumeCurrentGame,
  createNewGame,
} = useDirectorStart()

const hasAccount = computed(
  () => auth.isLoggedIn && auth.user != null && !auth.user.isGuest
)
</script>

<template>
  <section>
    <h1>Direttore di gioco</h1>
    <p class="text-muted">Crea una nuova partita. I giocatori si uniranno tramite QR.</p>
    <AlreadyInGamePrompt
      v-if="error && isAlreadyInGameError"
      :message="error"
      :loading="loading"
      @resume="resumeCurrentGame"
      @create-new="createNewGame"
    />
    <BaseAlert v-else-if="error">{{ error }}</BaseAlert>
    <div class="card app-main--narrow" style="margin: 0 auto">
      <div class="field">
        <label>Nome partita</label>
        <input v-model="gameName" class="input" :disabled="loading" />
      </div>

      <div class="home-panel__actions">
        <BaseButton
          v-if="hasCurrentGame"
          block
          variant="secondary"
          :disabled="loading"
          @click="resumeCurrentGame"
        >
          Torna alla partita in corso
        </BaseButton>
        <BaseButton
          v-if="!auth.isLoggedIn"
          block
          :disabled="loading"
          @click="startAsGuestDirector"
        >
          Avvia partita
        </BaseButton>

        <BaseButton
          v-else
          block
          :disabled="loading"
          @click="createGameWithCurrentUser"
        >
          {{ hasAccount ? 'Crea partita' : 'Avvia partita' }}
        </BaseButton>
      </div>
    </div>
  </section>
</template>
