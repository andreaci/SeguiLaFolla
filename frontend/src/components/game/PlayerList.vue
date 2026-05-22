<script setup lang="ts">
import type { PlayerState } from '../../types'

defineProps<{
  players: PlayerState[]
  showStatus?: boolean
}>()
</script>

<template>
  <ul class="player-list">
    <li v-for="p in players" :key="p.userId">
      <div>
        <span class="player-list__name">{{ p.displayName }}</span>
        <span v-if="showStatus" class="player-list__meta">
          <template v-if="p.answeredThisRound"> · ha risposto</template>
          <template v-if="p.hasVoted"> · ha votato</template>
        </span>
      </div>
      <span class="player-list__meta">
        {{ p.score }} pt
        <template v-if="p.penalties > 0"> · {{ p.penalties }} pen.</template>
      </span>
    </li>
  </ul>
</template>
