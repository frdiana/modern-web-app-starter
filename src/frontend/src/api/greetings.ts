import { apiFetch } from "../auth/Authentication"

export type Greeting = {
  id: string
  message: string
  createdAt: string
}

export async function listGreetings(signal: AbortSignal): Promise<Greeting[]> {
  const response = await apiFetch("/api/examples/greetings", { signal })

  if (!response.ok) {
    throw new Error(`API returned ${response.status}`)
  }

  return response.json() as Promise<Greeting[]>
}