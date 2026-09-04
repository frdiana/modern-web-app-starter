import { NavLink, Navigate, Route, Routes } from "react-router-dom"
import {
  AccountControl,
  ProtectedRoute,
} from "./auth/Authentication"
import { AboutPage } from "./routes/AboutPage"
import { HomePage } from "./routes/HomePage"

export default function App() {
  return (
    <div className="app-shell">
      <header className="site-header">
        <NavLink className="brand" to="/" aria-label="ModernWebApp home">
          <img src="/mark.svg" alt="" />
          <span>ModernWebApp</span>
        </NavLink>

        <nav aria-label="Main navigation">
          <NavLink to="/" end>
            Home
          </NavLink>
          <NavLink to="/about">About</NavLink>
          <AccountControl />
        </nav>
      </header>

      <main>
        <Routes>
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <HomePage />
              </ProtectedRoute>
            }
          />
          <Route path="/about" element={<AboutPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  )
}