<script setup lang="ts">
defineProps<{
  size?: 'normal' | 'large'
}>()
</script>

<template>
  <span class="penalty-poop" role="img" aria-label="Penalità attiva" title="Penalità attiva">
    <svg
      :class="{ 'penalty-poop__svg--large': size === 'large' }"
      class="penalty-poop__svg"
      viewBox="0 0 64 64"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <ellipse class="penalty-poop__shadow" cx="32" cy="52" rx="22" ry="8" />
      <path
        class="penalty-poop__body"
        d="M18 38 C14 28 18 14 28 12 C32 8 40 10 44 16 C52 14 56 24 52 34 C58 40 54 50 42 52 C38 58 26 58 22 50 C14 48 12 42 18 38 Z"
      />
      <ellipse class="penalty-poop__shine" cx="26" cy="30" rx="5" ry="6" />
      <circle class="penalty-poop__eye" cx="24" cy="32" r="3" />
      <circle class="penalty-poop__eye" cx="40" cy="32" r="3" />
      <path
        class="penalty-poop__mouth"
        d="M28 40 Q32 44 36 40"
        fill="none"
      />
      <ellipse class="penalty-poop__lump" cx="30" cy="18" rx="6" ry="7" />
      <ellipse class="penalty-poop__lump" cx="40" cy="14" rx="5" ry="6" />
    </svg>
  </span>
</template>

<style scoped>
.penalty-poop {
  display: inline-flex;
  align-items: center;
  vertical-align: middle;
  margin-right: 0.2rem;
  line-height: 0;
}

.penalty-poop__svg {
  width: 1.35em;
  height: 1.35em;
  filter: drop-shadow(2px 2px 0 var(--color-border));
  animation: penalty-poop-wiggle 2.5s ease-in-out infinite;
}

.penalty-poop__shadow {
  fill: var(--color-accent);
  opacity: 0.2;
}

.penalty-poop__body {
  fill: var(--color-accent);
  stroke: var(--color-border);
  stroke-width: 3;
  stroke-linejoin: round;
}

.penalty-poop__svg--large {
  width: 10rem !important;
  height: 10rem !important;
}

.penalty-poop__shine {
  fill: #fff;
  opacity: 0.45;
}

.penalty-poop__eye {
  fill: var(--color-border);
}

.penalty-poop__mouth {
  stroke: var(--color-border);
  stroke-width: 2.5;
  stroke-linecap: round;
}

.penalty-poop__lump {
  fill: var(--color-accent-hover);
  stroke: var(--color-border);
  stroke-width: 2.5;
}

@keyframes penalty-poop-wiggle {
  0%,
  100% {
    transform: rotate(-6deg) scale(1);
  }
  50% {
    transform: rotate(6deg) scale(1.08);
  }
}
</style>
