<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import QRCode from 'qrcode'

const props = defineProps<{
  url: string
  label?: string
  hoverZoom?: boolean
}>()

const dataUrl = ref('')

async function render() {
  if (!props.url) return
  dataUrl.value = await QRCode.toDataURL(props.url, { margin: 2, width: 240 })
}

onMounted(render)
watch(() => props.url, render)
</script>

<template>
  <div
    class="qr-panel card card--flat"
    :class="{ 'qr-panel--hover-zoom': hoverZoom }"
  >
    <p v-if="label" class="qr-panel__label text-muted">{{ label }}</p>
    <div class="qr-panel__frame">
      <img v-if="dataUrl" :src="dataUrl" alt="QR partita" class="qr-panel__img" />
    </div>
    <p class="qr-panel__url text-muted">{{ url }}</p>
  </div>
</template>

<style scoped>
.qr-panel {
  text-align: center;
}

.qr-panel__label {
  margin: 0 0 var(--space-sm);
  font-weight: 700;
}

.qr-panel__frame {
  display: inline-block;
  line-height: 0;
  overflow: visible;
}

.qr-panel__img {
  display: block;
  max-width: 100%;
  width: 220px;
  border-radius: var(--radius-sm);
  background: #fff;
  padding: var(--space-sm);
  border: 3px solid var(--color-border);
  box-shadow: var(--shadow);
}

.qr-panel--hover-zoom .qr-panel__frame {
  cursor: zoom-in;
}

.qr-panel--hover-zoom .qr-panel__img {
  transition: transform 0.25s ease, box-shadow 0.25s ease;
  transform-origin: center center;
}

.qr-panel--hover-zoom .qr-panel__frame:hover .qr-panel__img {
  transform: scale(1.35);
  box-shadow: 6px 6px 0 var(--color-border);
  position: relative;
  z-index: 2;
}

.qr-panel__url {
  margin: var(--space-md) 0 0;
  font-size: 0.75rem;
  word-break: break-all;
}
</style>
