<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{ phase: string }>()

const label = computed(() => {
  const map: Record<string, string> = {
    lobby: 'Lobby',
    answering: 'Risposte',
    voting: 'Voti',
    results: 'Risultati',
    finished: 'Fine',
  }
  return map[props.phase] ?? props.phase
})
</script>

<template>
  <span class="badge" :class="`badge--${phase}`">{{ label }}</span>
</template>
