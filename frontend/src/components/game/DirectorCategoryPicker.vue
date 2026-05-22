<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { api } from '../../api/client'
import type { QuestionCategory } from '../../types'

const model = defineModel<string | null>({ default: null })

const props = defineProps<{
  gameId: string
  disabled?: boolean
  gamePhase?: string
  compact?: boolean
}>()

const categories = ref<QuestionCategory[]>([])
const loading = ref(true)
const loadError = ref<string | null>(null)

async function load() {
  loading.value = true
  loadError.value = null
  try {
    categories.value = await api.getQuestionCategories(props.gameId)
  } catch (e) {
    loadError.value = (e as Error).message
    categories.value = []
  } finally {
    loading.value = false
  }
}

function select(id: string | null) {
  if (props.disabled) return
  model.value = id
}

function isSelected(id: string | null) {
  return (model.value ?? null) === id
}

onMounted(load)
watch(() => props.gameId, load)
watch(
  () => props.gamePhase,
  (phase) => {
    if (phase === 'lobby' || phase === 'results') load()
  }
)

defineExpose({ reload: load })
</script>

<template>
  <div class="category-picker" :class="{ 'category-picker--compact': compact }">
    
    <p v-if="loading" class="category-picker__hint text-muted">Caricamento categorie…</p>
    <p v-else-if="loadError" class="category-picker__hint text-muted">{{ loadError }}</p>
    <div v-else class="category-picker__grid" role="group" aria-label="Scegli categoria">
      <button
        type="button"
        class="category-picker__chip"
        :class="{ 'category-picker__chip--active': isSelected(null) }"
        :disabled="disabled"
        @click="select(null)"
      >
        <span class="category-picker__chip-label">Tutte</span>

      </button>
      <button
        v-for="cat in categories"
        :key="cat.id"
        type="button"
        class="category-picker__chip"
        :class="{
          'category-picker__chip--active': isSelected(cat.id),
          'category-picker__chip--empty': cat.available === 0,
        }"
        :disabled="disabled || cat.available === 0"
        @click="select(cat.id)"
      >
        <span class="category-picker__chip-label">{{ cat.label }} ({{ cat.available }})</span>

      </button>
    </div>
  </div>
</template>

<style scoped>
.category-picker--compact {
  margin-bottom: 0;
  padding: var(--space-sm) 0 0;
  background: transparent;
  border: none;
}

.category-picker {
  margin-bottom: var(--space-lg);
  padding: var(--space-md);
  background: var(--color-bg);
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
}

.category-picker__title {
  margin: 0 0 var(--space-sm);
  font-size: 0.95rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.category-picker__hint {
  margin: 0;
  font-size: 0.9rem;
  font-weight: 600;
}

.category-picker__grid {
  display: flex;
  flex-wrap: wrap;
  gap: var(--space-sm);
}

.category-picker__chip {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.1rem;
  padding: 0.5rem 0.75rem;
  border: 3px solid var(--color-border);
  border-radius: var(--radius-sm);
  background: var(--color-surface);
  font-family: inherit;
  cursor: pointer;
  box-shadow: 2px 2px 0 var(--color-border);
  transition: background 0.15s, transform 0.1s;
}

.category-picker__chip:hover:not(:disabled):not(.category-picker__chip--active) {
  background: var(--color-accent-soft);
}

.category-picker__chip--active {
  background: var(--color-accent);
  box-shadow: 3px 3px 0 var(--color-border);
}

.category-picker__chip:disabled,
.category-picker__chip--empty {
  opacity: 0.45;
  cursor: not-allowed;
  box-shadow: none;
}

.category-picker__chip-label {
  font-weight: 800;
  font-size: 0.95rem;
  text-transform: capitalize;
}

.category-picker__chip-meta {
  font-size: 0.75rem;
  font-weight: 600;
}
</style>
