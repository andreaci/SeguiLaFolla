<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useGameStore } from '../stores/game'
import { api } from '../api/client'
import PhaseBadge from '../components/ui/PhaseBadge.vue'
import BaseAlert from '../components/ui/BaseAlert.vue'
import QuestionDisplay from '../components/game/QuestionDisplay.vue'
import AnswerForm from '../components/game/AnswerForm.vue'
import RoundResultModal from '../components/game/RoundResultModal.vue'
import PlayerName from '../components/game/PlayerName.vue'
import PenaltyPoop from '../components/ui/PenaltyPoop.vue'
import { hasActivePenalty } from '../utils/penalty'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const gameStore = useGameStore()
const gameId = route.params.id as string
const busy = ref(false)
const submittedAnswerText = ref<string | null>(null)

const game = computed(() => gameStore.state)

const roundKey = computed(
  () => `${game.value?.phase ?? ''}-${game.value?.currentQuestion?.id ?? ''}`
)

const hasAnswered = computed(() => {
  const g = game.value
  if (!g || g.phase !== 'answering') return false
  if (submittedAnswerText.value) return true
  if (g.hasSubmittedAnswer || g.myAnswerId) return true
  const me = g.players.find((p) => p.userId === g.myUserId)
  return me?.answeredThisRound ?? me?.hasSubmittedAnswer ?? false
})

const myScore = computed(() => {
  const g = game.value
  if (!g?.myUserId) return 0
  return g.players.find((p) => p.userId === g.myUserId)?.score ?? 0
})

const iHavePoopPenalty = computed(() => {
  const g = game.value
  if (!g?.myUserId) return false
  const me = g.players.find((p) => p.userId === g.myUserId)
  if (me?.hasActivePenalty) return true
  return hasActivePenalty(g.myUserId, g.activePenaltyUserId)
})

function resetLocalRound() {
  submittedAnswerText.value = null
}

watch(roundKey, resetLocalRound)

onMounted(async () => {
  if (!auth.isLoggedIn) {
    router.replace(`/entra/${gameId}`)
    return
  }
  if (auth.user?.currentGameId && auth.user.currentGameId !== gameId) {
    try {
      await api.joinGame(gameId)
      await auth.loadMe()
    } catch {
      router.replace(`/entra/${gameId}`)
      return
    }
  } else if (!auth.user?.currentGameId) {
    try {
      await api.joinGame(gameId)
      await auth.loadMe()
    } catch {
      router.replace(`/entra/${gameId}`)
      return
    }
  }
  await gameStore.subscribe(gameId)
  if (gameStore.state?.isDirector) {
    router.replace(`/direttore/${gameId}`)
  }
})

watch(
  () => gameStore.state?.isDirector,
  (isDirector) => {
    if (isDirector) router.replace(`/direttore/${gameId}`)
  }
)

onUnmounted(() => gameStore.unsubscribe())

async function submitAnswer(text: string) {
  busy.value = true
  try {
    submittedAnswerText.value = text
    gameStore.state = await api.submitAnswer(gameId, text)
  } catch (e) {
    submittedAnswerText.value = null
    gameStore.error = (e as Error).message
  } finally {
    busy.value = false
  }
}
</script>

<template>
  <section v-if="game" class="app-main--narrow" style="margin: 0 auto">
    <div style="display: flex; justify-content: space-between; align-items: center">
      <h1 style="font-size: 1.25rem">{{ game.name }}</h1>
      <PhaseBadge :phase="game.phase" />
    </div>

    <p class="play__identity text-muted text-center">
      <PlayerName
        v-if="auth.user && game.myUserId"
        :name="auth.user.displayName"
        :user-id="game.myUserId"
        :active-penalty-user-id="game.activePenaltyUserId"
        :show-poop="iHavePoopPenalty"
      />
      <template v-else>{{ auth.user?.displayName }}</template>
      · {{ myScore }} pt
    </p>

    <div v-if="iHavePoopPenalty" class="play__poop-banner card" role="status">
      <PenaltyPoop size="large" /><br />
      <span>Hai la penalità della foll(i)a!</span>
    </div>

    <BaseAlert v-if="gameStore.error">{{ gameStore.error }}</BaseAlert>

    <div v-if="game.phase === 'lobby'" class="card text-center">
      <p>In attesa che il direttore avvii il turno…</p>
    </div>

    <template v-else-if="game.currentQuestion">
      <QuestionDisplay
        :key="roundKey"
        :question="game.currentQuestion"
        :show-choices="false"
      />

      <div v-if="game.phase === 'answering'" class="card">
        <AnswerForm
          :key="`answer-${roundKey}`"
          :question="game.currentQuestion"
          :disabled="busy"
          :waiting="hasAnswered"
          :submitted-text="submittedAnswerText"
          @submit="submitAnswer"
        />
      </div>

      <p v-else-if="game.phase === 'results'" class="text-muted text-center play__waiting-results">
        Guarda il risultato del turno…
      </p>

      <p v-else-if="game.phase === 'finished'" class="card text-center">
        Partita terminata. Grazie per aver giocato!
      </p>
    </template>
  </section>
  <p v-else class="text-muted text-center">Caricamento…</p>

  <RoundResultModal v-if="game?.phase === 'results'" :game="game" />
</template>

<style scoped>
.play__identity {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
  gap: 0.15rem;
  font-weight: 700;
}

.play__waiting-results {
  margin-top: var(--space-md);
}

.play__poop-banner {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-sm);
  margin-bottom: var(--space-md);
  padding: var(--space-md);
  background: #fff8e8;
  border: 3px dashed var(--color-border);
  font-weight: 800;
  text-align: center;
}

.play__poop-banner :deep(.penalty-poop__svg) {
  width: 2rem;
  height: 2rem;
}
</style>
