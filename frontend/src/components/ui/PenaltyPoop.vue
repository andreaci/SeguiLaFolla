<template>
  <span class="penalty-poop" role="img" aria-label="Penalità attiva" title="Penalità attiva">
    <svg
      class="penalty-poop__svg"
      viewBox="0 0 64 64"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <ellipse cx="32" cy="52" rx="22" ry="8" fill="#8B5A2B" opacity="0.25" />
      <path
        d="M18 38 C14 28 18 14 28 12 C32 8 40 10 44 16 C52 14 56 24 52 34 C58 40 54 50 42 52 C38 58 26 58 22 50 C14 48 12 42 18 38 Z"
        fill="#A0522D"
        stroke="#0d0d0d"
        stroke-width="3"
        stroke-linejoin="round"
      />
      <ellipse cx="26" cy="30" rx="5" ry="6" fill="#fff" opacity="0.35" />
      <circle cx="24" cy="32" r="3" fill="#0d0d0d" />
      <circle cx="40" cy="32" r="3" fill="#0d0d0d" />
      <path
        d="M28 40 Q32 44 36 40"
        fill="none"
        stroke="#0d0d0d"
        stroke-width="2.5"
        stroke-linecap="round"
      />
      <ellipse cx="30" cy="18" rx="6" ry="7" fill="#8B4513" stroke="#0d0d0d" stroke-width="2.5" />
      <ellipse cx="40" cy="14" rx="5" ry="6" fill="#8B4513" stroke="#0d0d0d" stroke-width="2.5" />
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
