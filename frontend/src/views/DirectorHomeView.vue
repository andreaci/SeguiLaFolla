<script setup lang="ts">
import { computed } from 'vue'
import { useDirectorStart } from '../composables/useDirectorStart'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'

const { auth, gameName, error, loading, startAsGuestDirector, createGameWithCurrentUser } =
  useDirectorStart()

const hasAccount = computed(
  () => auth.isLoggedIn && auth.user != null && !auth.user.isGuest
)
</script>

<template>
  <section>
    <h1>Direttore di gioco</h1>
    <p class="text-muted">Crea una nuova partita. I giocatori si uniranno tramite QR.</p>
    <BaseAlert v-if="error">{{ error }}</BaseAlert>
    <div class="card app-main--narrow" style="margin: 0 auto">
      <div class="field">
        <label>Nome partita</label>
        <input v-model="gameName" class="input" :disabled="loading" />
      </div>

      <div class="home-panel__actions">
        <BaseButton
          v-if="!auth.isLoggedIn"
          block
          :disabled="loading"
          @click="startAsGuestDirector"
        >
          Genera direttore e avvia partita
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

      <p v-if="!auth.isLoggedIn" class="text-muted text-center" style="margin-top: 1rem">
        <router-link to="/accedi">Accedi</router-link>
        se vuoi usare un account registrato
      </p>
    </div>
  </section>
</template>
