import { computed, ref, watch } from 'vue'
import type { Ref } from 'vue'
import type { GameState } from '../types'
import { hasActivePenalty } from '../utils/penalty'

/** Splash quando il giocatore riceve la penalità cacca in questo turno (fase results). */
export function usePenaltyPoopSplash(game: Ref<GameState | null | undefined>) {
  const showPoopSplash = ref(false)
  const lastSplashKey = ref<string | null>(null)

  const splashTriggerKey = computed(() => {
    const g = game.value
    if (!g?.myUserId || g.phase !== 'results' || !g.lastRoundResult) return null

    const r = g.lastRoundResult
    if (!r.penaltyUserId || !hasActivePenalty(g.myUserId, r.penaltyUserId)) return null

    return [
      r.penaltyUserId,
      r.winningAnswerId ?? r.winningAnswerText ?? 'tie',
      r.penaltyReason ?? '',
    ].join('|')
  })

  watch(splashTriggerKey, (key) => {
    if (!key || key === lastSplashKey.value) return
    lastSplashKey.value = key
    showPoopSplash.value = true
  })

  function onSplashDone() {
    showPoopSplash.value = false
  }

  return { showPoopSplash, onSplashDone }
}
