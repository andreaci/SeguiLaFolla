const IN_ANOTHER_GAME_HINT = "un'altra partita"

export function isInAnotherGameError(message: string | null | undefined): boolean {
  return (message ?? '').toLowerCase().includes(IN_ANOTHER_GAME_HINT)
}
