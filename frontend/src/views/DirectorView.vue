<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useGameStore } from '../stores/game'
import { api } from '../api/client'
import PhaseBadge from '../components/ui/PhaseBadge.vue'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import PlayerList from '../components/game/PlayerList.vue'
import QuestionDisplay from '../components/game/QuestionDisplay.vue'
import RoundResult from '../components/game/RoundResult.vue'
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
  <section v-if="game">
    <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem">
      <div>
        <h1>{{ game.name }}</h1>
        <PhaseBadge :phase="game.phase" />
      </div>
      <QrCodePanel :url="joinUrl" label="QR per i giocatori" />
    </div>

    <BaseAlert v-if="gameStore.error">{{ gameStore.error }}</BaseAlert>

    <div class="grid-2" style="margin-top: 1.5rem">
      <div class="card">
        <h2>Giocatori</h2>
        <PlayerList :players="game.players" show-status />
      </div>

      <div class="card">
        <h2>Turno</h2>
        <QuestionDisplay
          v-if="game.currentQuestion"
          :key="`${game.phase}-${game.currentQuestion.id}`"
          :question="game.currentQuestion"
        />

        <ul v-if="game.phase === 'answering'" class="player-list">
          <li v-for="a in game.answers" :key="a.text + (a.authorName ?? '')">
            <span>{{ a.authorName ?? '?' }}</span>
            <span class="text-muted">{{ a.text }}</span>
          </li>
        </ul>

        <ul v-if="game.phase === 'results'" class="player-list">
          <li v-for="a in game.answers.filter(x => x.id)" :key="a.id">
            <span>{{ a.text }}</span>
            <span class="text-muted">
              {{ a.voteCount }} {{ a.voteCount === 1 ? 'voto' : 'voti' }}
              <template v-if="a.authorName"> · {{ a.authorName }}</template>
            </span>
          </li>
        </ul>

        <RoundResult v-if="game.phase === 'results'" :game="game" />

        <div class="actions-row">
          <BaseButton v-if="game.phase === 'lobby' || game.phase === 'results'" @click="startRound">
            {{ game.phase === 'results' ? 'Prossima domanda' : 'Inizia turno' }}
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
    </div>
  </section>
  <p v-else class="text-muted">Caricamento partita…</p>
</template>
