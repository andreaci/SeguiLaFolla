<script setup lang="ts">
import type { PlayerState } from '../../types'
import PlayerName from './PlayerName.vue'

defineProps<{
  players: PlayerState[]
  showStatus?: boolean
  activePenaltyUserId?: string | null
}>()
</script>

<template>
  <ul class="player-list">
    <li v-for="p in players" :key="p.userId">
      <div>
        <PlayerName
          class="player-list__name"
          :name="p.displayName"
          :user-id="p.userId"
          :active-penalty-user-id="activePenaltyUserId"
        />
        <span v-if="showStatus" class="player-list__meta">
          <template v-if="p.answeredThisRound"> · ha risposto</template>
          <template v-if="p.hasVoted"> · ha votato</template>
        </span>
      </div>
      <span class="player-list__meta">{{ p.score }} pt</span>
    </li>
  </ul>
</template>
