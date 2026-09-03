import { apiFetch } from "../auth/Authentication"

export type EchoResponse = {
  message: string
  timestamp: string
}

export async function getGreeting(signal: AbortSignal): Promise<EchoResponse> {
  const query = new URLSearchParams({ message: "Hello World" })
  const response = await apiFetch(`/api/examples/echo?${query}`, { signal })

  if (!response.ok) {
    throw new Error(`API returned ${response.status}`)
  }

  return response.json() as Promise<EchoResponse>
}