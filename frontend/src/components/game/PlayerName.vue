<script setup lang="ts">
import PenaltyPoop from '../ui/PenaltyPoop.vue'
import { computed } from 'vue'
import { hasActivePenalty } from '../../utils/penalty'

const props = defineProps<{
  name: string
  userId: string
  activePenaltyUserId?: string | null
  showPoop?: boolean
}>()

const show = computed(
  () => props.showPoop ?? hasActivePenalty(props.userId, props.activePenaltyUserId)
)
</script>

<template>
  <span class="player-name">
    <PenaltyPoop v-if="show" />
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
