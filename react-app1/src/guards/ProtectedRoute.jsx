import { Navigate } from "react-router-dom"
import { useAuth } from "../context/AuthContext"

export default function ProtectedRoute({ children, allowedRoles }) {
  const { user, profile, loading } = useAuth()

  // ✅ WAIT until data is ready
  if (loading || !user || !profile) {
    return <div>Loading...</div>
  }

  // ✅ SAFE ROLE CHECK
  if (!allowedRoles.includes(profile.role)) {
    return <Navigate to="/unauthorized" />
  }

  return children
}