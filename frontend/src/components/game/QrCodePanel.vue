<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import QRCode from 'qrcode'

const props = defineProps<{ url: string; label?: string }>()
const dataUrl = ref('')

async function render() {
  if (!props.url) return
  dataUrl.value = await QRCode.toDataURL(props.url, { margin: 2, width: 240 })
}

onMounted(render)
watch(() => props.url, render)
</script>

<template>
  <div class="qr-panel card card--flat">
    <p v-if="label" class="text-muted">{{ label }}</p>
    <img v-if="dataUrl" :src="dataUrl" alt="QR partita" />
    <p class="text-muted" style="font-size: 0.8rem; word-break: break-all">{{ url }}</p>
  </div>
</template>
