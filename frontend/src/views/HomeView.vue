<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useDirectorStart } from '../composables/useDirectorStart'
import { hideAuthButtons } from '../config/features'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import AlreadyInGamePrompt from '../components/game/AlreadyInGamePrompt.vue'

const router = useRouter()
const {
  auth,
  error,
  loading,
  hasCurrentGame,
  isAlreadyInGameError,
  resumeCurrentGame,
  createNewGame,
} = useDirectorStart()
</script>

<template>
  <section class="home">
    <header class="home__hero" >
      <h1 class="home__title">Segui la foll(i)a</h1>
      <p class="home__tagline text-muted">
        Il gioco da tavolo: rispondi come la mandria, vota la risposta più popolare e guadagna punti.
      </p>
    </header>

    <AlreadyInGamePrompt
      v-if="error && isAlreadyInGameError"
      :message="error"
      :loading="loading"
      @resume="resumeCurrentGame"
      @create-new="createNewGame"
    />
    <BaseAlert v-else-if="error">{{ error }}</BaseAlert>

    <div class="home__columns">
      <div class="home__panel card home__panel--director">
        <h2 class="home__panel-title">Avvia partita</h2>
        <p class="text-muted">
          Sei il direttore? Crea la partita e mostra il QR ai giocatori. Non serve un account.
        </p>
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
            v-if="!auth.isLoggedIn && !hideAuthButtons"
            block
            variant="secondary"
            @click="router.push('/accedi')"
          >
            Accedi
          </BaseButton>
          <BaseButton
            v-if="!auth.isLoggedIn && !hideAuthButtons"
            block
            variant="secondary"
            @click="router.push('/registrati')"
          >
            Registrati
          </BaseButton>

         
          <BaseButton block :disabled="loading" @click="router.push('/direttore')">
            Avvia partita
          </BaseButton>
        </div>
      </div>

      <div class="home__panel card home__panel--player">
        <h2 class="home__panel-title">Giocatore</h2>
        <p class="text-muted">
          Scansiona con il telefono il QR della partita o apri il link che ti ha dato il direttore.
        </p>
      </div>
    </div>
  </section>
</template>

<style scoped>
.home {
  max-width: var(--max-width-wide);
  margin: 0 auto;
}

.home__hero {
  text-align: center;
  margin-bottom: var(--space-xl);
}

.home__title {
  font-size: clamp(2rem, 6vw, 3rem);
  font-weight: 900;
  margin-bottom: var(--space-sm);
  letter-spacing: -0.02em;
}

.home__tagline {
  max-width: 36rem;
  margin: 0 auto;
  font-size: 1.05rem;
}

.home__columns {
  display: grid;
  gap: var(--space-lg);
}

@media (min-width: 720px) {
  .home__columns {
    grid-template-columns: 1fr 1fr;
  }
}

.home__panel-title {
  font-size: 1.5rem;
  font-weight: 900;
  margin-bottom: var(--space-sm);
  padding-bottom: var(--space-sm);
  border-bottom: 3px solid var(--color-border);
}

.home__panel--director {
  background: var(--color-accent-soft);
}

.home__panel--player {
  background: var(--color-bg);
}

.home__signed-in {
  margin: 0;
  padding: var(--space-md);
  border: 3px dashed var(--color-border);
  border-radius: var(--radius-sm);
  text-align: center;
  font-weight: 600;
}
</style>
