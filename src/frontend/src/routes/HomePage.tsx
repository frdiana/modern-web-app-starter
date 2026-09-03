import { useEffect, useState } from "react"
import { getGreeting, type EchoResponse } from "../api/echo"

type RequestState =
  | { status: "loading" }
  | { status: "success"; data: EchoResponse }
  | { status: "error"; message: string }

export function HomePage() {
  const [request, setRequest] = useState<RequestState>({ status: "loading" })

  useEffect(() => {
    const controller = new AbortController()

    getGreeting(controller.signal)
      .then((data) => setRequest({ status: "success", data }))
      .catch((error: unknown) => {
        if (error instanceof DOMException && error.name === "AbortError") {
          return
        }

        const message = error instanceof Error ? error.message : "Unknown error"
        setRequest({ status: "error", message })
      })

    return () => controller.abort()
  }, [])

  return (
    <section className="home-page">
      <p className="eyebrow">React + ASP.NET Core + Aspire</p>
      <h1>A small starting point.</h1>
      <p className="intro">
        Routing is ready and this page calls the backend through Aspire.
      </p>

      <div className="api-result" aria-live="polite">
        <span className={`status-dot status-${request.status}`} aria-hidden="true" />
        <div>
          <p className="result-label">API response</p>
          {request.status === "loading" && <p>Connecting to the API...</p>}
          {request.status === "success" && (
            <>
              <p className="greeting">{request.data.message}</p>
              <p className="timestamp">
                Received {new Date(request.data.timestamp).toLocaleString()}
              </p>
            </>
          )}
          {request.status === "error" && (
            <p className="error-message">Could not reach the API: {request.message}</p>
          )}
        </div>
      </div>
    </section>
  )
}