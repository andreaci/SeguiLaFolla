<script setup lang="ts">
import { onMounted, ref } from 'vue'

const emit = defineEmits<{ done: [] }>()

const phase = ref<'fly' | 'splat' | 'out'>('fly')
const visible = ref(true)

onMounted(() => {
  const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches
  const flyMs = reducedMotion ? 80 : 650
  const splatMs = reducedMotion ? 120 : 900
  const outMs = reducedMotion ? 200 : 550

  setTimeout(() => {
    phase.value = 'splat'
  }, flyMs)

  setTimeout(() => {
    phase.value = 'out'
  }, flyMs + splatMs)

  setTimeout(() => {
    visible.value = false
    emit('done')
  }, flyMs + splatMs + outMs)
})

function dismiss() {
  visible.value = false
  emit('done')
}
</script>

<template>
  <Teleport to="body">
    <div
      v-if="visible"
      class="poop-splash"
      :class="`poop-splash--${phase}`"
      role="dialog"
      aria-modal="true"
      aria-label="Penalità! Ti è stata lanciata la cacca"
      @click="dismiss"
    >
      <div class="poop-splash__flash" aria-hidden="true" />
      <div class="poop-splash__splats" aria-hidden="true">
        <span v-for="n in 8" :key="n" class="poop-splash__splat" />
      </div>

      <div class="poop-splash__poop-wrap">
        <svg
          class="poop-splash__poop"
          viewBox="0 0 64 64"
          xmlns="http://www.w3.org/2000/svg"
          aria-hidden="true"
        >
          <ellipse class="poop-splash__shadow" cx="32" cy="52" rx="22" ry="8" />
          <path
            class="poop-splash__body"
            d="M18 38 C14 28 18 14 28 12 C32 8 40 10 44 16 C52 14 56 24 52 34 C58 40 54 50 42 52 C38 58 26 58 22 50 C14 48 12 42 18 38 Z"
          />
          <ellipse class="poop-splash__shine" cx="26" cy="30" rx="5" ry="6" />
          <circle class="poop-splash__eye" cx="24" cy="32" r="3" />
          <circle class="poop-splash__eye" cx="40" cy="32" r="3" />
          <path class="poop-splash__mouth" d="M28 40 Q32 44 36 40" fill="none" />
          <ellipse class="poop-splash__lump" cx="30" cy="18" rx="6" ry="7" />
          <ellipse class="poop-splash__lump" cx="40" cy="14" rx="5" ry="6" />
        </svg>
      </div>

      <p class="poop-splash__caption">CACCA!</p>
      <p class="poop-splash__sub">Hai la penalità della foll(i)a</p>
      <p class="poop-splash__hint text-muted">Tocca per chiudere</p>
    </div>
  </Teleport>
</template>

<style scoped>
.poop-splash {
  position: fixed;
  inset: 0;
  z-index: 3000;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  cursor: pointer;
  background: rgba(13, 13, 13, 0.35);
  animation: poop-splash-bg-in 0.25s ease-out forwards;
}

.poop-splash--out {
  animation: poop-splash-bg-out 0.55s ease-in forwards;
}

.poop-splash__flash {
  position: absolute;
  inset: 0;
  background: radial-gradient(
    circle at 50% 45%,
    var(--color-accent-soft) 0%,
    transparent 55%
  );
  opacity: 0;
  pointer-events: none;
}

.poop-splash--splat .poop-splash__flash,
.poop-splash--out .poop-splash__flash {
  animation: poop-splash-flash 0.45s ease-out forwards;
}

.poop-splash__splats {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.poop-splash__splat {
  position: absolute;
  width: clamp(2rem, 8vw, 4rem);
  height: clamp(1.5rem, 6vw, 3rem);
  border-radius: 50% 50% 45% 55%;
  background: var(--color-accent);
  border: 3px solid var(--color-border);
  opacity: 0;
  transform: scale(0);
}

.poop-splash--splat .poop-splash__splat {
  animation: poop-splash-splat-pop 0.5s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.poop-splash__splat:nth-child(1) {
  top: 12%;
  left: 8%;
  animation-delay: 0.05s;
}
.poop-splash__splat:nth-child(2) {
  top: 18%;
  right: 10%;
  animation-delay: 0.1s;
}
.poop-splash__splat:nth-child(3) {
  bottom: 22%;
  left: 12%;
  animation-delay: 0.08s;
}
.poop-splash__splat:nth-child(4) {
  bottom: 15%;
  right: 8%;
  animation-delay: 0.12s;
}
.poop-splash__splat:nth-child(5) {
  top: 42%;
  left: 4%;
  animation-delay: 0.15s;
}
.poop-splash__splat:nth-child(6) {
  top: 38%;
  right: 5%;
  animation-delay: 0.07s;
}
.poop-splash__splat:nth-child(7) {
  bottom: 38%;
  left: 42%;
  animation-delay: 0.18s;
  transform: scale(0) rotate(25deg);
}
.poop-splash__splat:nth-child(8) {
  top: 8%;
  left: 45%;
  animation-delay: 0.04s;
}

.poop-splash__poop-wrap {
  position: relative;
  z-index: 2;
  filter: drop-shadow(8px 8px 0 var(--color-border));
  transform: translate(120vw, -90vh) rotate(-540deg) scale(0.15);
}

.poop-splash--fly .poop-splash__poop-wrap {
  animation: poop-splash-throw 0.65s cubic-bezier(0.22, 1, 0.36, 1) forwards;
}

.poop-splash--splat .poop-splash__poop-wrap {
  animation: poop-splash-land 0.35s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.poop-splash--out .poop-splash__poop-wrap {
  animation: poop-splash-exit 0.55s ease-in forwards;
}

.poop-splash__poop {
  width: min(72vw, 22rem);
  height: min(72vw, 22rem);
}

.poop-splash__shadow {
  fill: var(--color-accent);
  opacity: 0.25;
}

.poop-splash__body {
  fill: var(--color-accent);
  stroke: var(--color-border);
  stroke-width: 3;
  stroke-linejoin: round;
}

.poop-splash__shine {
  fill: #fff;
  opacity: 0.45;
}

.poop-splash__eye {
  fill: var(--color-border);
}

.poop-splash__mouth {
  stroke: var(--color-border);
  stroke-width: 2.5;
  stroke-linecap: round;
}

.poop-splash__lump {
  fill: var(--color-accent-hover);
  stroke: var(--color-border);
  stroke-width: 2.5;
}

.poop-splash__caption {
  position: relative;
  z-index: 2;
  margin: var(--space-md) 0 0;
  font-size: clamp(2.5rem, 12vw, 4.5rem);
  font-weight: 900;
  letter-spacing: 0.08em;
  color: var(--color-text);
  text-shadow: 4px 4px 0 var(--color-accent);
  opacity: 0;
  transform: scale(0.5);
}

.poop-splash--splat .poop-splash__caption {
  animation: poop-splash-text-pop 0.4s 0.15s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.poop-splash__sub {
  position: relative;
  z-index: 2;
  margin: var(--space-sm) 0 0;
  font-size: clamp(1rem, 4vw, 1.35rem);
  font-weight: 800;
  opacity: 0;
}

.poop-splash--splat .poop-splash__sub {
  animation: poop-splash-text-pop 0.4s 0.25s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.poop-splash__hint {
  position: relative;
  z-index: 2;
  margin-top: var(--space-xl);
  font-size: 0.85rem;
  font-weight: 600;
  opacity: 0;
}

.poop-splash--splat .poop-splash__hint {
  animation: poop-splash-fade-in 0.35s 0.6s ease-out forwards;
}

@keyframes poop-splash-throw {
  0% {
    transform: translate(120vw, -90vh) rotate(-540deg) scale(0.15);
  }
  70% {
    transform: translate(-4vw, 4vh) rotate(15deg) scale(1.08);
  }
  100% {
    transform: translate(0, 0) rotate(-8deg) scale(1);
  }
}

@keyframes poop-splash-land {
  0% {
    transform: translate(0, 0) rotate(-8deg) scale(1);
  }
  40% {
    transform: translate(0, 0) rotate(6deg) scale(1.18);
  }
  100% {
    transform: translate(0, 0) rotate(-4deg) scale(1.05);
  }
}

@keyframes poop-splash-exit {
  to {
    transform: translate(0, 0) rotate(12deg) scale(0.2);
    opacity: 0;
  }
}

@keyframes poop-splash-flash {
  0% {
    opacity: 0;
  }
  30% {
    opacity: 0.95;
  }
  100% {
    opacity: 0.35;
  }
}

@keyframes poop-splash-splat-pop {
  to {
    opacity: 0.85;
    transform: scale(1) rotate(var(--rot, 0deg));
  }
}

@keyframes poop-splash-text-pop {
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes poop-splash-fade-in {
  to {
    opacity: 0.75;
  }
}

@keyframes poop-splash-bg-in {
  from {
    background: rgba(13, 13, 13, 0);
  }
}

@keyframes poop-splash-bg-out {
  to {
    background: rgba(13, 13, 13, 0);
  }
}

@media (prefers-reduced-motion: reduce) {
  .poop-splash--fly .poop-splash__poop-wrap {
    animation: poop-splash-land 0.12s ease-out forwards;
  }

  .poop-splash__splats {
    display: none;
  }
}
</style>
