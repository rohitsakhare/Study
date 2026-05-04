import { Routes, Route } from 'react-router-dom';
import Login from './Login';
import Signup from './Signup';
import Dashboard from './Dashboard';
import ProtectedRoute from './routes/ProtectedRoute';
import { useAuth } from "./context/AuthContext";
import FullScreenLoader from "./components/FullScreenLoader";

function App() {
  const { authLoading } = useAuth();

  if (authLoading) return <FullScreenLoader />;

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/signup" element={<Signup />} />

      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        }
      />
    </Routes>
  );
}

export default App;