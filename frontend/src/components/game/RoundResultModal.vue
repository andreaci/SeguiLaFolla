<script setup lang="ts">
import { computed, ref } from 'vue'
import type { GameState } from '../../types'
import BaseButton from '../ui/BaseButton.vue'
import PenaltyPoop from '../ui/PenaltyPoop.vue'
import PlayerName from './PlayerName.vue'
import DirectorCategoryPicker from './DirectorCategoryPicker.vue'
import { hasActivePenalty, playerHasPoop } from '../../utils/penalty'

const selectedCategory = defineModel<string | null>('selectedCategory', { default: null })

const props = defineProps<{
  game: GameState
  gameId: string
  showNextButton?: boolean
  loading?: boolean
}>()

const categoryPickerRef = ref<InstanceType<typeof DirectorCategoryPicker> | null>(null)

const emit = defineEmits<{ next: [] }>()

function onNext() {
  emit('next')
  categoryPickerRef.value?.reload()
}

defineExpose({ reloadCategories: () => categoryPickerRef.value?.reload() })

const result = computed(() => props.game.lastRoundResult)

const winnerIds = computed(() => result.value?.scoredVoterIds ?? [])

const penaltyPlayer = computed(() => {
  const id = props.game.activePenaltyUserId ?? result.value?.penaltyUserId
  if (!id) return null
  return props.game.players.find((p) => p.userId === id) ?? null
})

const roundPenaltyApplied = computed(
  () =>
    !!result.value?.penaltyUserId &&
    hasActivePenalty(result.value.penaltyUserId, props.game.activePenaltyUserId)
)
</script>

<template>
  <div
    v-if="result && game.phase === 'results'"
    class="round-modal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="round-modal-title"
  >
    <div class="round-modal__backdrop" aria-hidden="true" />
    <div class="round-modal__panel card">
      <h2 id="round-modal-title" class="round-modal__title">Risultato turno</h2>

      <div v-if="result.isTie" class="round-modal__hero round-modal__hero--tie">
        <p class="round-modal__hero-label">Pareggio</p>
        <p class="round-modal__hero-text">Nessuno prende punti</p>
        <p class="round-modal__hero-sub text-muted">
          Due o più risposte hanno lo stesso numero di voti.
        </p>
      </div>

      <div v-else-if="result.winningAnswerText" class="round-modal__hero">
        <p class="round-modal__hero-label">Risposta vincente</p>
        <p class="round-modal__answer">{{ result.winningAnswerText }}</p>
        <p class="round-modal__votes text-muted">
          {{ result.winningVoteCount }}
          {{ result.winningVoteCount === 1 ? 'voto' : 'voti' }}
        </p>
      </div>

      <div v-if="!result.isTie && winnerIds.length" class="round-modal__section">
        <h3 class="round-modal__section-title">Hanno risposto così</h3>
        <ul class="round-modal__winners">
          <li v-for="id in winnerIds" :key="id">
            <PlayerName
              :name="game.players.find((p) => p.userId === id)?.displayName ?? '?'"
              :user-id="id"
              :active-penalty-user-id="game.activePenaltyUserId"
              :show-poop="playerHasPoop(
                game.players.find((p) => p.userId === id) ?? { userId: id },
                game.activePenaltyUserId
              )"
            />
            <span class="round-modal__plus">+1 pt</span>
          </li>
        </ul>
      </div>

      <div
        v-if="penaltyPlayer && game.activePenaltyUserId"
        class="round-modal__section round-modal__penalty"
      >
        <h3 class="round-modal__section-title">Penalità</h3>
        <div class="round-modal__penalty-row">
          <PenaltyPoop />
          <span class="round-modal__penalty-name">{{ penaltyPlayer.displayName }}</span>
        </div>
        <p v-if="result.penaltyReason && roundPenaltyApplied" class="round-modal__penalty-reason text-muted">
          {{ result.penaltyReason }}
        </p>
        <p
          v-else-if="game.activePenaltyUserId && !roundPenaltyApplied"
          class="round-modal__penalty-reason text-muted"
        >
          Porta ancora la penalità fino al prossimo evento.
        </p>
      </div>

      <div class="round-modal__actions">
        <BaseButton v-if="showNextButton" block :disabled="loading" @click="onNext">
          Prossima domanda
        </BaseButton>
        <DirectorCategoryPicker
          v-if="showNextButton"
          ref="categoryPickerRef"
          v-model="selectedCategory"
          :game-id="gameId"
          game-phase="results"
          :disabled="loading"
          compact
        />
        <p v-else class="round-modal__wait text-muted text-center">
          In attesa della prossima domanda…
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.round-modal {
  position: fixed;
  inset: 0;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-md);
}

.round-modal__backdrop {
  position: absolute;
  inset: 0;
  background: rgba(13, 13, 13, 0.55);
}

.round-modal__panel {
  position: relative;
  z-index: 1;
  width: min(100%, 520px);
  max-height: min(90vh, 640px);
  overflow-y: auto;
  margin: 0;
  padding: var(--space-xl);
  border-width: 4px;
  box-shadow: 8px 8px 0 var(--color-border);
}

.round-modal__title {
  margin: 0 0 var(--space-lg);
  font-size: clamp(1.75rem, 5vw, 2.25rem);
  font-weight: 900;
  text-align: center;
}

.round-modal__hero {
  text-align: center;
  padding: var(--space-lg);
  margin-bottom: var(--space-lg);
  background: var(--color-accent-soft);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
}

.round-modal__hero--tie {
  background: var(--color-bg);
}

.round-modal__hero-label {
  margin: 0 0 var(--space-sm);
  font-size: 0.85rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.round-modal__hero-text {
  margin: 0;
  font-size: clamp(1.5rem, 5vw, 2rem);
  font-weight: 900;
}

.round-modal__hero-sub {
  margin: var(--space-sm) 0 0;
  font-size: 0.95rem;
  font-weight: 600;
}

.round-modal__answer {
  margin: 0;
  font-size: clamp(1.35rem, 4.5vw, 1.85rem);
  font-weight: 900;
  line-height: 1.25;
}

.round-modal__votes {
  margin: var(--space-sm) 0 0;
  font-weight: 700;
}

.round-modal__section {
  margin-bottom: var(--space-lg);
}

.round-modal__section-title {
  margin: 0 0 var(--space-sm);
  font-size: 0.95rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.round-modal__winners {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}

.round-modal__winners li {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-md);
  background: var(--color-bg);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
  font-weight: 800;
}

.round-modal__plus {
  color: var(--color-accent);
  font-weight: 900;
}

.round-modal__penalty {
  padding: var(--space-md);
  background: #fff8e8;
  border: 3px dashed var(--color-border);
  border-radius: var(--radius-sm);
}

.round-modal__penalty-row {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
  font-size: 1.15rem;
  font-weight: 800;
}

.round-modal__penalty-row :deep(.penalty-poop) {
  margin-right: 0;
}

.round-modal__penalty-row :deep(.penalty-poop__svg) {
  width: 2.5rem;
  height: 2.5rem;
}

.round-modal__penalty-name {
  font-size: 1.15rem;
  font-weight: 900;
}

.round-modal__penalty-reason {
  margin: var(--space-sm) 0 0;
  font-size: 0.9rem;
  font-weight: 600;
}

.round-modal__actions {
  margin-top: var(--space-lg);
  position: relative;
  z-index: 2;
}

.round-modal__wait {
  margin: 0;
  font-weight: 700;
}
</style>
