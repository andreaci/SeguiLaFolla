<script setup lang="ts">
defineProps<{ variant?: 'error' | 'info' }>()
</script>

<template>
  <div class="alert" :class="`alert--${variant ?? 'error'}`">
    <slot />
  </div>
</template>
