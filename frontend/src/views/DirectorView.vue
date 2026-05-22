<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useGameStore } from '../stores/game'
import { api } from '../api/client'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import PlayerList from '../components/game/PlayerList.vue'
import QuestionDisplay from '../components/game/QuestionDisplay.vue'
import RoundResultModal from '../components/game/RoundResultModal.vue'
import DirectorRoundTracker from '../components/game/DirectorRoundTracker.vue'
import QrCodePanel from '../components/game/QrCodePanel.vue'
const route = useRoute()
const router = useRouter()
const gameStore = useGameStore()
const gameId = route.params.id as string

const game = computed(() => gameStore.state)
const joinUrl = computed(() => `${window.location.origin}/entra/${gameId}`)

onMounted(async () => {
  if (!localStorage.getItem('mandria_token')) {
    router.replace('/direttore')
    return
  }
  await gameStore.subscribe(gameId)
})
onUnmounted(() => gameStore.unsubscribe())

async function startRound() {
  await gameStore.run(() => api.startRound(gameId))
}

async function forceEndTurn() {
  await gameStore.run(() => api.forceEndTurn(gameId))
}

const canForceEndTurn = computed(() => game.value?.phase === 'answering')

async function toLobby() {
  await gameStore.run(() => api.toLobby(gameId))
}
</script>

<template>
  <section v-if="game" class="director">
    <header class="director__header" >
      <h1 class="director__title">{{ game.name }}</h1>
    </header>

    <BaseAlert v-if="gameStore.error">{{ gameStore.error }}</BaseAlert>

    <div class="director__grid">
      <div class="director__main card">
        <h2 class="director__section-title">Domanda attuale</h2>
        <QuestionDisplay
          v-if="game.currentQuestion"
          :key="`${game.phase}-${game.currentQuestion.id}`"
          :question="game.currentQuestion"
          show-choices
        />
        <p v-else class="text-muted">Nessuna domanda in corso.</p>

        <DirectorRoundTracker v-if="game.phase === 'answering'" :game="game" />

        <div class="actions-row">
          <BaseButton v-if="game.phase === 'lobby'" @click="startRound">
            Inizia turno
          </BaseButton>
          <BaseButton
            v-if="canForceEndTurn"
            variant="danger"
            @click="forceEndTurn"
          >
            Fine turno
          </BaseButton>
          <BaseButton v-if="game.phase === 'results'" variant="secondary" @click="toLobby">
            Torna in lobby
          </BaseButton>
        </div>
      </div>

      <aside class="director__aside">
        <QrCodePanel :url="joinUrl" label="QR per i giocatori" hover-zoom />

        <div class="card director__players-card">
          <h2 class="director__section-title">Giocatori</h2>
          <PlayerList
            :players="game.players"
            show-status
            :active-penalty-user-id="game.activePenaltyUserId"
          />
        </div>
      </aside>
    </div>
  </section>
  <p v-else class="text-muted">Caricamento partita…</p>

  <RoundResultModal
    v-if="game?.phase === 'results'"
    :game="game"
    show-next-button
    @next="startRound"
  />
</template>

<style scoped>
.director {
  width: 100%;
  max-width: var(--max-width-wide);
  margin: 0 auto;
}

.director__header {
  text-align: center;
  margin-bottom: var(--space-xl);
  padding-bottom: var(--space-md);
  border-bottom: 3px solid var(--color-border);
}

.director__title {
  font-size: clamp(2.25rem, 6vw, 3.75rem);
  font-weight: 900;
  line-height: 1.1;
  margin: 0 0 var(--space-md);
  letter-spacing: -0.02em;
}

.director__grid {
  display: grid;
  gap: var(--space-lg);
  align-items: start;
}

@media (min-width: 768px) {
  .director__grid {
    grid-template-columns: 2fr 1fr;
  }
}

.director__main {
  min-width: 0;
}

.director__aside {
  display: flex;
  flex-direction: column;
  gap: var(--space-lg);
  min-width: 0;
}

.director__section-title {
  font-size: 1.15rem;
  font-weight: 800;
  margin: 0 0 var(--space-md);
}

.director__player-status {
  margin-top: var(--space-lg);
  padding: var(--space-md);
  background: var(--color-bg);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
}

.director__subsection-title {
  margin: 0 0 var(--space-sm);
  font-size: 1rem;
  font-weight: 800;
}

.director__players-card {
  flex: 1;
}
</style>
