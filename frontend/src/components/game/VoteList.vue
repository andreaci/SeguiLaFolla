<script setup lang="ts">
import type { AnswerState } from '../../types'

const props = defineProps<{
  answers: AnswerState[]
  disabled?: boolean
  hasVoted?: boolean
}>()

const emit = defineEmits<{ vote: [id: string] }>()

function onVote(a: AnswerState) {
  if (props.disabled || props.hasVoted || a.isMine || !a.id || a.id === '00000000-0000-0000-0000-000000000000')
    return
  emit('vote', a.id)
}
</script>

<template>
  <div class="vote-list">
    <button
      v-for="a in answers.filter(x => x.id && x.id !== '00000000-0000-0000-0000-000000000000')"
      :key="a.id"
      type="button"
      class="vote-item"
      :class="{
        'vote-item--mine': a.isMine,
        'vote-item--disabled': disabled || hasVoted,
      }"
      @click="onVote(a)"
    >
      <span>
        {{ a.text }}
        <span v-if="a.revealAuthor && a.authorName" class="text-muted">
          — {{ a.authorName }}
        </span>
      </span>
      <span v-if="a.voteCount > 0" class="text-muted">{{ a.voteCount }} voti</span>
    </button>
  </div>
  <p v-if="hasVoted" class="text-muted text-center">Hai già votato. In attesa degli altri…</p>
</template>
