<script setup lang="ts">
import type { QuestionDto } from '../../types'

defineProps<{ question: QuestionDto }>()
</script>

<template>
  <div class="question-box">
    <span class="text-muted" style="font-size: 0.85rem">
      {{ question.tipo === 'multipla' ? 'Scelta multipla' : 'Domanda aperta' }}
    </span>
    <p style="margin: 0.5rem 0 0">{{ question.domanda }}</p>
  </div>
</template>
