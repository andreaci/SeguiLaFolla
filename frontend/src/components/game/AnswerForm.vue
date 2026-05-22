<script setup lang="ts">
import { ref, watch } from 'vue'
import type { QuestionDto } from '../../types'
import BaseButton from '../ui/BaseButton.vue'

const props = defineProps<{
  question: QuestionDto
  disabled?: boolean
  waiting?: boolean
  submittedText?: string | null
}>()

const emit = defineEmits<{ submit: [text: string] }>()

const text = ref('')
const locked = () => props.disabled || props.waiting

watch(
  () => props.submittedText,
  (v) => {
    if (v) text.value = v
  },
  { immediate: true }
)

function pickOption(opt: string) {
  if (locked()) return
  emit('submit', opt)
}

function submitOpen() {
  if (locked() || !text.value.trim()) return
  emit('submit', text.value.trim())
}
</script>

<template>
  <div
    class="answer-form"
    :class="{ 'answer-form--waiting': waiting }"
  >
    <p v-if="waiting" class="answer-form__status" role="status">
      Attendendo gli altri giocatori…
    </p>

    <div v-if="question.tipo === 'multipla'" class="option-grid">
      <button
        v-for="opt in question.opzioni"
        :key="opt"
        type="button"
        class="option-btn"
        :class="{
          'option-btn--selected': waiting && submittedText === opt,
          'option-btn--locked': waiting,
        }"
        :disabled="locked()"
        @click="pickOption(opt)"
      >
        {{ opt }}
      </button>
    </div>
    <form v-else @submit.prevent="submitOpen">
      <div class="field">
        <label for="answer">La tua risposta</label>
        <input
          id="answer"
          v-model="text"
          class="input"
          type="text"
          maxlength="200"
          :disabled="locked()"
          placeholder="Scrivi qui..."
          autocomplete="off"
        />
      </div>
      <BaseButton
        type="submit"
        :disabled="locked() || (!waiting && !text.trim())"
        block
      >
        {{ waiting ? 'Risposta inviata' : 'Invia risposta' }}
      </BaseButton>
    </form>
  </div>
</template>
