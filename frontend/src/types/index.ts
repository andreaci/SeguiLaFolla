export interface UserInfo {
  id: string
  displayName: string
  username?: string
  isGuest: boolean
  currentGameId?: string
}

export interface AuthResponse {
  token: string
  user: UserInfo
}

export interface QuestionDto {
  id: number
  tipo: 'aperta' | 'multipla'
  domanda: string
  opzioni?: string[]
}

export interface PlayerState {
  userId: string
  displayName: string
  score: number
  penalties: number
  hasSubmittedAnswer: boolean
  hasVoted: boolean
  answeredThisRound: boolean
}

export interface AnswerState {
  id: string
  text: string
  voteCount: number
  isMine: boolean
  authorUserId?: string
  authorName?: string
  revealAuthor: boolean
}

export interface RoundResult {
  isTie?: boolean
  winningAnswerId?: string
  winningAnswerText?: string
  winningVoteCount: number
  scoredVoterIds: string[]
  penaltyUserId?: string
  penaltyReason?: string
  voteCountsByAnswer: Record<string, number>
}

export interface GameState {
  id: string
  name: string
  phase: 'lobby' | 'answering' | 'voting' | 'results' | 'finished'
  currentQuestion?: QuestionDto
  players: PlayerState[]
  answers: AnswerState[]
  lastRoundResult?: RoundResult
  activePenaltyUserId?: string
  isDirector: boolean
  myUserId?: string
  hasSubmittedAnswer: boolean
  hasVoted: boolean
  myAnswerId?: string
}

export interface GameSummary {
  id: string
  name: string
  phase: string
  playerCount: number
}
