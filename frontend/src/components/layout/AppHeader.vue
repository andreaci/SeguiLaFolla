<script setup lang="ts">
import { useAuthStore } from '../../stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

function logout() {
  auth.logout()
  router.push('/')
}
</script>

<template>
  <header class="header" style="display:none">
    <router-link to="/" class="header__brand">🐄 Segui la foll(i)a</router-link>
    <div class="header__actions">
      <span v-if="auth.user" class="header__user">{{ auth.user.displayName }}</span>
      <button v-if="auth.user" class="btn btn--secondary btn--sm" @click="logout">
        Esci
      </button>
    </div>
  </header>
</template>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--space-md) var(--space-lg);
  margin-bottom: var(--space-lg);
  background: var(--color-bg);
  border-bottom: 3px solid var(--color-border);
}

.header__brand {
  font-weight: 900;
  font-size: 1.15rem;
  color: var(--color-text);
  text-decoration: none;
}

.header__brand:hover {
  color: var(--color-accent);
}

.header__user {
  font-weight: 700;
  font-size: 0.9rem;
}

.header__actions {
  display: flex;
  align-items: center;
  gap: var(--space-md);
}
</style>
