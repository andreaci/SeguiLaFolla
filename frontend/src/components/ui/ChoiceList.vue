<script setup lang="ts">
defineProps<{
  options: string[]
  disabled?: boolean
  selected?: string | null
}>()

const emit = defineEmits<{ select: [value: string] }>()

function onPick(opt: string) {
  emit('select', opt)
}
</script>

<template>
  <ul class="numbered-list">
    <li v-for="(opt, index) in options" :key="opt" class="numbered-list__row">
      <button
        type="button"
        class="numbered-list__item numbered-list__item--interactive"
        :class="{
          'numbered-list__item--selected': selected === opt,
          'numbered-list__item--locked': disabled,
        }"
        :disabled="disabled"
        @click="onPick(opt)"
      >
        <span class="numbered-list__badge">{{ index + 1 }}</span>
        <span class="numbered-list__primary">{{ opt }}</span>
      </button>
    </li>
  </ul>
</template>
