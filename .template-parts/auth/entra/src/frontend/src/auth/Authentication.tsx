import {
  InteractionRequiredAuthError,
  PublicClientApplication,
  type Configuration,
} from "@azure/msal-browser"
import {
  MsalProvider,
  useIsAuthenticated,
  useMsal,
} from "@azure/msal-react"
import type { PropsWithChildren } from "react"

const tenantId = import.meta.env.VITE_ENTRA_TENANT_ID ?? "YOUR_TENANT_ID"
const clientId = import.meta.env.VITE_ENTRA_CLIENT_ID ?? "YOUR_SPA_CLIENT_ID"
const apiClientId = import.meta.env.VITE_ENTRA_API_CLIENT_ID ?? "YOUR_API_CLIENT_ID"
const apiScopeName = import.meta.env.VITE_ENTRA_API_SCOPE ?? "access_as_user"
const redirectUri = import.meta.env.VITE_ENTRA_REDIRECT_URI ?? "http://localhost:5173"
const apiScope = `api://${apiClientId}/${apiScopeName}`

const configuration: Configuration = {
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri,
    postLogoutRedirectUri: redirectUri,
  },
  cache: {
    cacheLocation: "sessionStorage",
  },
}

const client = new PublicClientApplication(configuration)

export function AuthenticationProvider({ children }: PropsWithChildren) {
  return <MsalProvider instance={client}>{children}</MsalProvider>
}

export function ProtectedRoute({ children }: PropsWithChildren) {
  const isAuthenticated = useIsAuthenticated()
  const { instance, inProgress } = useMsal()

  if (inProgress !== "none") {
    return <p className="auth-message">Completing sign in...</p>
  }

  if (!isAuthenticated) {
    return (
      <section className="auth-page">
        <p className="eyebrow">Authentication required</p>
        <h1>Sign in to continue.</h1>
        <button
          className="action-button"
          type="button"
          onClick={() => void instance.loginRedirect({ scopes: [apiScope] })}
        >
          Sign in with Microsoft
        </button>
      </section>
    )
  }

  return children
}

export function AccountControl() {
  const isAuthenticated = useIsAuthenticated()
  const { instance } = useMsal()

  if (!isAuthenticated) {
    return null
  }

  return (
    <button
      className="nav-button"
      type="button"
      onClick={() => void instance.logoutRedirect()}
    >
      Sign out
    </button>
  )
}

export async function apiFetch(input: RequestInfo | URL, init?: RequestInit) {
  const account = client.getActiveAccount() ?? client.getAllAccounts()[0]

  if (!account) {
    await client.loginRedirect({ scopes: [apiScope] })
    throw new Error("Redirecting to sign in.")
  }

  let accessToken: string

  try {
    const result = await client.acquireTokenSilent({ account, scopes: [apiScope] })
    accessToken = result.accessToken
  } catch (error: unknown) {
    if (error instanceof InteractionRequiredAuthError) {
      await client.acquireTokenRedirect({ account, scopes: [apiScope] })
      throw new Error("Redirecting to acquire an access token.")
    }

    throw error
  }

  const headers = new Headers(init?.headers)
  headers.set("Authorization", `Bearer ${accessToken}`)

  return fetch(input, { ...init, headers })
}