import type { PropsWithChildren } from "react"

export function AuthenticationProvider({ children }: PropsWithChildren) {
  return children
}

export function ProtectedRoute({ children }: PropsWithChildren) {
  return children
}

export function AccountControl() {
  return null
}

export function apiFetch(input: RequestInfo | URL, init?: RequestInit) {
  return fetch(input, init)
}