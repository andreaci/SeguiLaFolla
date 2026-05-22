<script setup lang="ts">
import { computed } from 'vue'
import type { QuestionDto } from '../../types'
import NumberedList from '../ui/NumberedList.vue'
import {
  getQuestionOptions,
  isMultiplaQuestion,
  questionText,
} from '../../utils/question'

const props = defineProps<{
  question: QuestionDto
  showChoices?: boolean
}>()

const multipla = computed(() => isMultiplaQuestion(props.question))
const options = computed(() => getQuestionOptions(props.question))
const showChoices = computed(
  () => props.showChoices !== false && multipla.value && options.value.length > 0
)

const choiceItems = computed(() =>
  options.value.map((opt, i) => ({
    id: `opt-${i}-${opt}`,
    primary: opt,
  }))
)
</script>

<template>
  <div class="question-box">
    <span class="question-box__type text-muted">
      {{ multipla ? 'Scelta multipla' : 'Domanda aperta' }}
    </span>
    <p class="question-box__text">{{ questionText(question) }}</p>

    <div v-if="showChoices" class="question-box__choices">
      <p class="question-box__choices-label">Possibili risposte</p>
      <NumberedList :items="choiceItems" />
    </div>
  </div>
</template>

<style scoped>
.question-box__type {
  font-size: 0.85rem;
  font-weight: 700;
}

.question-box__text {
  margin: 0.5rem 0 0;
  font-size: 1.25rem;
  font-weight: 700;
}

.question-box__choices {
  margin-top: var(--space-lg);
  padding-top: var(--space-md);
  border-top: 2px dashed var(--color-border);
}

.question-box__choices-label {
  margin: 0 0 var(--space-sm);
  font-size: 0.9rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}
</style>
