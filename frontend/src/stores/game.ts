import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '../api/client'
import { joinGameHub, leaveGameHub } from '../api/signalr'
import type { GameState } from '../types'

export const useGameStore = defineStore('game', () => {
  const state = ref<GameState | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  let subscribedId: string | null = null

  async function refresh(id: string) {
    loading.value = true
    error.value = null
    try {
      state.value = await api.getGame(id)
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      loading.value = false
    }
  }

  async function subscribe(id: string) {
    if (subscribedId && subscribedId !== id) {
      await leaveGameHub(subscribedId)
    }
    subscribedId = id
    await joinGameHub(id, () => refresh(id))
    await refresh(id)
  }

  async function unsubscribe() {
    if (subscribedId) {
      await leaveGameHub(subscribedId)
      subscribedId = null
    }
    state.value = null
  }

  async function run<T>(fn: () => Promise<T>) {
    loading.value = true
    error.value = null
    try {
      const result = await fn()
      const id = (result as GameState | undefined)?.id ?? state.value?.id
      if (id) await refresh(id)
      return result
    } catch (e) {
      error.value = (e as Error).message
      throw e
    } finally {
      loading.value = false
    }
  }

  return { state, loading, error, refresh, subscribe, unsubscribe, run }
})
