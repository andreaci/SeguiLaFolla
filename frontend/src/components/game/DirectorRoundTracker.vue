<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { GameState, PlayerState } from '../../types'
import PlayerName from './PlayerName.vue'

const props = defineProps<{
  game: GameState
}>()

type FilterMode = 'pending' | 'all'

const filterMode = ref<FilterMode>('pending')

watch(
  () => props.game.phase,
  () => {
    filterMode.value = 'pending'
  }
)

export interface TrackerRow {
  userId: string
  displayName: string
  answered: boolean
  answerText?: string
}

const pendingCount = computed(() =>
  props.game.players.filter((p) => !p.answeredThisRound).length
)

const answerByUserId = computed(() => {
  const map = new Map<string, string>()
  for (const a of props.game.answers) {
    if (!a.text) continue
    if (a.authorUserId) {
      map.set(a.authorUserId, a.text)
      continue
    }
    const player = props.game.players.find((p) => p.displayName === a.authorName)
    if (player) map.set(player.userId, a.text)
  }
  return map
})

function buildRow(p: PlayerState): TrackerRow {
  const answered = p.answeredThisRound
  return {
    userId: p.userId,
    displayName: p.displayName,
    answered,
    answerText: answered ? answerByUserId.value.get(p.userId) : undefined,
  }
}

const allRows = computed(() => props.game.players.map(buildRow))

const visibleRows = computed(() => {
  if (filterMode.value === 'all') return allRows.value
  return allRows.value.filter((r) => !r.answered)
})

function setFilter(mode: FilterMode) {
  filterMode.value = mode
}
</script>

<template>
  <section class="round-tracker" aria-labelledby="round-tracker-title">
    <div class="round-tracker__filter" role="group" aria-label="Filtro giocatori">
      <button
        type="button"
        class="round-tracker__filter-btn"
        :class="{ 'round-tracker__filter-btn--active': filterMode === 'pending' }"
        @click="setFilter('pending')"
      >
        Da rispondere
      </button>
      <button
        type="button"
        class="round-tracker__filter-btn"
        :class="{ 'round-tracker__filter-btn--active': filterMode === 'all' }"
        @click="setFilter('all')"
      >
        Mostra tutti
      </button>
    </div>

    <div class="round-tracker__count-block" :class="{ 'round-tracker__count-block--done': pendingCount === 0 }">
      <p class="round-tracker__count-label">
        {{ pendingCount === 0 ? 'Tutti hanno risposto' : 'Devono ancora rispondere' }}
      </p>
      <p class="round-tracker__count" aria-live="polite">{{ pendingCount }}</p>
    </div>

    <ul v-if="visibleRows.length" class="round-tracker__list">
      <li
        v-for="row in visibleRows"
        :key="row.userId"
        class="round-tracker__row"
        :class="{ 'round-tracker__row--pending': !row.answered }"
      >
        <PlayerName
          class="round-tracker__name"
          :name="row.displayName"
          :user-id="row.userId"
          :active-penalty-user-id="game.activePenaltyUserId"
        />
        <span v-if="row.answered && row.answerText" class="round-tracker__answer">
          {{ row.answerText }}
        </span>
        <span v-else-if="row.answered" class="round-tracker__answer round-tracker__answer--muted">
          Risposta inviata
        </span>
        <span v-else class="round-tracker__waiting">
          <span class="round-tracker__hourglass" aria-hidden="true">⏳</span>
          <span>In attesa</span>
        </span>
      </li>
    </ul>

    <p v-else class="round-tracker__empty">
      <template v-if="filterMode === 'pending'">Nessun giocatore in attesa.</template>
      <template v-else>Nessun giocatore in partita.</template>
    </p>
  </section>
</template>

<style scoped>
.round-tracker {
  margin-top: var(--space-lg);
  padding: var(--space-lg);
  background: var(--color-surface);
  border: 3px solid var(--color-border);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.round-tracker__filter {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-sm);
  padding: var(--space-xs);
  background: var(--color-bg);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
  margin-bottom: var(--space-lg);
}

.round-tracker__filter-btn {
  padding: 0.65rem 0.5rem;
  border: 2px solid transparent;
  border-radius: calc(var(--radius-sm) - 4px);
  background: transparent;
  font-family: inherit;
  font-size: clamp(0.75rem, 2.8vw, 0.9rem);
  font-weight: 800;
  color: var(--color-text);
  cursor: pointer;
  transition: background 0.15s, box-shadow 0.15s;
}

.round-tracker__filter-btn:hover:not(.round-tracker__filter-btn--active) {
  background: var(--color-accent-soft);
}

.round-tracker__filter-btn--active {
  background: var(--color-accent);
  border-color: var(--color-border);
  box-shadow: 2px 2px 0 var(--color-border);
}

.round-tracker__count-block {
  text-align: center;
  padding: var(--space-md) var(--space-lg) var(--space-lg);
  margin-bottom: var(--space-lg);
  background: var(--color-accent-soft);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
}

.round-tracker__count-block--done {
  background: var(--color-bg);
}

.round-tracker__count-label {
  margin: 0 0 var(--space-xs);
  font-size: 0.95rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.06em;
}

.round-tracker__count {
  margin: 0;
  font-size: clamp(4rem, 18vw, 6.5rem);
  font-weight: 900;
  line-height: 1;
  color: var(--color-accent);
  text-shadow: 3px 3px 0 var(--color-border);
}

.round-tracker__count-block--done .round-tracker__count {
  font-size: clamp(2.5rem, 10vw, 4rem);
  color: var(--color-text);
}

.round-tracker__list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}

.round-tracker__row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-md);
  padding: var(--space-md) var(--space-lg);
  background: var(--color-bg);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
  box-shadow: 2px 2px 0 var(--color-border);
}

.round-tracker__row--pending {
  background: var(--color-surface-alt);
  border-style: dashed;
}

.round-tracker__name {
  font-weight: 800;
  font-size: 1.05rem;
  flex-shrink: 0;
}

.round-tracker__answer {
  text-align: right;
  font-weight: 700;
  font-size: 0.95rem;
  line-height: 1.3;
  max-width: 55%;
  word-break: break-word;
}

.round-tracker__answer--muted {
  color: var(--color-muted);
  font-weight: 600;
}

.round-tracker__waiting {
  display: inline-flex;
  align-items: center;
  gap: var(--space-sm);
  font-weight: 700;
  font-size: 0.9rem;
  color: var(--color-muted);
}

.round-tracker__hourglass {
  font-size: 1.35rem;
  line-height: 1;
}

.round-tracker__empty {
  margin: 0;
  text-align: center;
  font-weight: 700;
  color: var(--color-muted);
  padding: var(--space-md);
}
</style>
