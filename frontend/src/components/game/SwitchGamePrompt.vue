<script setup lang="ts">
import BaseButton from '../ui/BaseButton.vue'

defineProps<{
  message: string
  loading?: boolean
}>()

const emit = defineEmits<{ switch: []; resume: [] }>()
</script>

<template>
  <div class="switch-game card" role="alert">
    <p class="switch-game__message">{{ message }}</p>
    <div class="switch-game__actions">
      <BaseButton block :disabled="loading" @click="emit('switch')">
        Nahhh, scappo e entro in questa
      </BaseButton>
      <BaseButton block variant="secondary" :disabled="loading" @click="emit('resume')">
        È vero, torno in quella!
      </BaseButton>
    </div>
  </div>
</template>

<style scoped>
.switch-game {
  padding: var(--space-lg);
  background: var(--color-accent-soft);
  border: 3px solid var(--color-border);
}

.switch-game__message {
  margin: 0 0 var(--space-md);
  font-weight: 700;
}

.switch-game__actions {
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}
</style>
