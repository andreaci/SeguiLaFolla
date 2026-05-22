<script setup lang="ts">
withDefaults(
  defineProps<{
    variant?: 'primary' | 'secondary' | 'danger'
    block?: boolean
    small?: boolean
    disabled?: boolean
    type?: 'button' | 'submit'
  }>(),
  { variant: 'primary', block: false, small: false, disabled: false, type: 'button' }
)
</script>

<template>
  <button
    :type="type"
    class="btn"
    :class="[
      `btn--${variant}`,
      { 'btn--block': block, 'btn--sm': small },
    ]"
    :disabled="disabled"
  >
    <slot />
  </button>
</template>
