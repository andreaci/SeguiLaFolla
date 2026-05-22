import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { api } from '../api/client'

const ALREADY_IN_GAME_HINT = 'già in una partita'

export function useDirectorStart() {
  const router = useRouter()
  const auth = useAuthStore()
  const gameName = ref('Mandria di stasera')
  const error = ref<string | null>(null)
  const loading = ref(false)

  const hasCurrentGame = computed(() => !!auth.user?.currentGameId)

  const isAlreadyInGameError = computed(() =>
    (error.value ?? '').toLowerCase().includes(ALREADY_IN_GAME_HINT)
  )

  onMounted(async () => {
    if (localStorage.getItem('mandria_token')) {
      await auth.loadMe()
    }
  })

  async function resumeCurrentGame() {
    const id = auth.user?.currentGameId
    if (!id) return
    loading.value = true
    error.value = null
    try {
      const state = await api.getGame(id)
      await router.push(state.isDirector ? `/direttore/${id}` : `/partita/${id}`)
    } catch (e) {
      error.value = (e as Error).message
      await auth.loadMe()
    } finally {
      loading.value = false
    }
  }

  async function leaveCurrentGame() {
    const id = auth.user?.currentGameId
    if (!id) return
    await api.leaveGame(id)
    await auth.loadMe()
  }

  async function createGameWithCurrentUser() {
    loading.value = true
    error.value = null
    try {
      const game = await api.createGame(gameName.value)
      await auth.loadMe()
      await router.push(`/direttore/${game.id}`)
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
      await auth.loadMe()
      await router.push(`/direttore/${game.id}`)
    } catch (e) {
      error.value = (e as Error).message
    } finally {
      loading.value = false
    }
  }

  /** Esce dalla partita attuale e ne crea una nuova. */
  async function createNewGame() {
    loading.value = true
    error.value = null
    try {
      if (auth.user?.currentGameId) {
        await leaveCurrentGame()
      }
      if (!auth.isLoggedIn) {
        const label = `Direttore ${Math.floor(1000 + Math.random() * 9000)}`
        await auth.guest(label)
      }
      const game = await api.createGame(gameName.value)
      await auth.loadMe()
      await router.push(`/direttore/${game.id}`)
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
    hasCurrentGame,
    isAlreadyInGameError,
    createGameWithCurrentUser,
    startAsGuestDirector,
    resumeCurrentGame,
    createNewGame,
  }
}
