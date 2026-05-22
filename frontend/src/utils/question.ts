import type { QuestionDto } from '../types'

type QuestionRaw = QuestionDto & {
  Opzioni?: string[]
  Tipo?: string
  Domanda?: string
}

export function questionTipo(q?: QuestionDto | null): string {
  if (!q) return ''
  const raw = q as QuestionRaw
  return (raw.tipo ?? raw.Tipo ?? '').toLowerCase()
}

export function isMultiplaQuestion(q?: QuestionDto | null): boolean {
  return questionTipo(q) === 'multipla'
}

export function getQuestionOptions(q?: QuestionDto | null): string[] {
  if (!q) return []
  const raw = q as QuestionRaw
  const list = raw.opzioni ?? raw.Opzioni
  return Array.isArray(list) ? list : []
}

export function questionText(q?: QuestionDto | null): string {
  if (!q) return ''
  const raw = q as QuestionRaw
  return raw.domanda ?? raw.Domanda ?? ''
}
