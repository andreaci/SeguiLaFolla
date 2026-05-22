<script setup lang="ts">
import PenaltyPoop from '../ui/PenaltyPoop.vue'
import { hasActivePenalty } from '../../utils/penalty'

defineProps<{
  name: string
  userId: string
  activePenaltyUserId?: string | null
}>()
</script>

<template>
  <span class="player-name">
    <PenaltyPoop v-if="hasActivePenalty(userId, activePenaltyUserId)" />
    <span class="player-name__text">{{ name }}</span>
  </span>
</template>

<style scoped>
.player-name {
  display: inline-flex;
  align-items: center;
  gap: 0.15rem;
}

.player-name__text {
  font-weight: inherit;
}
</style>
