<script setup lang="ts">
import { computed } from 'vue'
import type { GameState } from '../../types'

const props = defineProps<{ game: GameState }>()

const result = computed(() => props.game.lastRoundResult)
const penaltyName = computed(() => {
  const id = result.value?.penaltyUserId
  if (!id) return null
  return props.game.players.find(p => p.userId === id)?.displayName
})
const scorerNames = computed(() =>
  (result.value?.scoredVoterIds ?? [])
    .map(id => props.game.players.find(p => p.userId === id)?.displayName)
    .filter(Boolean)
)
</script>

<template>
  <div v-if="result" class="card">
    <h3>Risultato turno</h3>
    <p v-if="result.winningAnswerText">
      <strong>Risposta della mandria:</strong> {{ result.winningAnswerText }}
      <span class="text-muted">({{ result.winningVoteCount }} {{ result.winningVoteCount === 1 ? 'voto' : 'voti' }})</span>
    </p>
    <p v-if="scorerNames.length">
      <strong>+1 punto a chi ha risposto così:</strong> {{ scorerNames.join(', ') }}
    </p>
    <p v-if="penaltyName" class="alert alert--info">
      <strong>Penalità:</strong> {{ penaltyName }}
      <span v-if="result.penaltyReason"> — {{ result.penaltyReason }}</span>
    </p>
    <p v-if="result.penaltyReason && !penaltyName" class="text-muted">
      {{ result.penaltyReason }}
    </p>
  </div>
</template>
