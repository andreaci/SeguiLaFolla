import * as signalR from '@microsoft/signalr'
import { hubUrl } from '../config/api'

let connection: signalR.HubConnection | null = null

export function getHub(): signalR.HubConnection {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl(), { withCredentials: false })
      .withAutomaticReconnect()
      .build()
  }
  return connection
}

export async function joinGameHub(gameId: string, onUpdate: () => void) {
  const hub = getHub()
  hub.off('gameUpdated')
  hub.on('gameUpdated', onUpdate)

  if (hub.state === signalR.HubConnectionState.Disconnected) {
    await hub.start()
  }
  await hub.invoke('JoinGameGroup', gameId)
}

export async function leaveGameHub(gameId: string) {
  const hub = getHub()
  if (hub.state === signalR.HubConnectionState.Connected) {
    await hub.invoke('LeaveGameGroup', gameId)
  }
  hub.off('gameUpdated')
}
