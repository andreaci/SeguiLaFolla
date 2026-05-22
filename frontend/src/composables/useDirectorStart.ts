import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { api } from '../api/client'

export function useDirectorStart() {
  const router = useRouter()
  const auth = useAuthStore()
  const gameName = ref('Mandria di stasera')
  const error = ref<string | null>(null)
  const loading = ref(false)

  async function createGameWithCurrentUser() {
    loading.value = true
    error.value = null
    try {
      const game = await api.createGame(gameName.value)
      router.push(`/direttore/${game.id}`)
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      loading.value = false
    }
  }

  async function startAsGuestDirector() {
    loading.value = true
    error.value = null
    try {
      const label = `Direttore ${Math.floor(1000 + Math.random() * 9000)}`
      await auth.guest(label)
      const game = await api.createGame(gameName.value)
      router.push(`/direttore/${game.id}`)
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      loading.value = false
    }
  }

  return {
    auth,
    gameName,
    error,
    loading,
    createGameWithCurrentUser,
    startAsGuestDirector,
  }
}
