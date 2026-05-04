import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import FullScreenLoader from "../components/FullScreenLoader";

export default function ProtectedRoute({ children, role }) {
  const { user, profile, loading, profileLoading } = useAuth();

  // 1. Only block when auth is still initializing
  if (loading || (role && profileLoading)) {
    return <FullScreenLoader />;
  }

  // 2. If no user → redirect immediately
  if (!user) {
    return <Navigate to="/login" replace />;
  }

  // 4. Role check (safe fallback)
  if (role && profile && profile.role !== role) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}