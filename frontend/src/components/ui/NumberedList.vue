<script setup lang="ts">
export interface NumberedListItem {
  id: string
  primary: string
  secondary?: string
}

defineProps<{
  items: NumberedListItem[]
}>()
</script>

<template>
  <ul class="numbered-list">
    <li
      v-for="(item, index) in items"
      :key="item.id"
      class="numbered-list__item"
    >
      <span class="numbered-list__badge">{{ index + 1 }}</span>
      <div class="numbered-list__content">
        <span class="numbered-list__primary">{{ item.primary }}</span>
        <span v-if="item.secondary" class="numbered-list__secondary text-muted">
          {{ item.secondary }}
        </span>
      </div>
    </li>
  </ul>
</template>
