<script setup lang="ts">
import BaseButton from '../ui/BaseButton.vue'

defineProps<{
  message: string
  loading?: boolean
}>()

const emit = defineEmits<{
  resume: []
  createNew: []
}>()
</script>

<template>
  <div class="already-in-game card" role="alert">
    <p class="already-in-game__message">{{ message }}</p>
    <div class="already-in-game__actions">
      <BaseButton :disabled="loading" @click="emit('createNew')">
        Crea nuova
      </BaseButton>
      <BaseButton variant="secondary" :disabled="loading" @click="emit('resume')">
        Torna a quella
      </BaseButton>
    </div>
  </div>
</template>

<style scoped>
.already-in-game {
  margin-bottom: var(--space-lg);
  padding: var(--space-lg);
  background: var(--color-accent-soft);
  border: 3px solid var(--color-border);
}

.already-in-game__message {
  margin: 0 0 var(--space-md);
  font-weight: 700;
}

.already-in-game__actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-sm);
}

.already-in-game__actions .btn {
  flex: 1;
  min-width: 8rem;
}
</style>
