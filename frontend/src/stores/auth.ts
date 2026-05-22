import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api, setToken } from '../api/client'
import type { UserInfo } from '../types'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserInfo | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isLoggedIn = computed(() => !!user.value)

  async function loadMe() {
    try {
      user.value = await api.me()
    } catch {
      user.value = null
      setToken(null)
    }
  }

  async function applyAuth(token: string, u: UserInfo) {
    setToken(token)
    user.value = u
  }

  async function register(username: string, password: string, displayName: string) {
    loading.value = true
    error.value = null
    try {
      const res = await api.register(username, password, displayName)
      await applyAuth(res.token, res.user)
    } catch (e) {
      error.value = (e as Error).message
      throw e
    } finally {
      loading.value = false
    }
  }

  async function login(username: string, password: string) {
    loading.value = true
    error.value = null
    try {
      const res = await api.login(username, password)
      await applyAuth(res.token, res.user)
    } catch (e) {
      error.value = (e as Error).message
      throw e
    } finally {
      loading.value = false
    }
  }

  async function guest(displayName: string) {
    loading.value = true
    error.value = null
    try {
      const res = await api.guest(displayName)
      await applyAuth(res.token, res.user)
    } catch (e) {
      error.value = (e as Error).message
      throw e
    } finally {
      loading.value = false
    }
  }

  function logout() {
    setToken(null)
    user.value = null
  }

  return { user, loading, error, isLoggedIn, loadMe, register, login, guest, logout }
})
