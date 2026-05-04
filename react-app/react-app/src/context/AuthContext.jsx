import { createContext, useContext, useEffect, useState } from "react";
import { supabase } from "../supabaseClient";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  // ======================
  // STATE
  // ======================
  const [session, setSession] = useState(null);
  const [user, setUser] = useState(null);
  const [profile, setProfile] = useState(null);

  const [authLoading, setAuthLoading] = useState(true);
  const [profileLoading, setProfileLoading] = useState(false);

  // ======================
  // FETCH PROFILE
  // ======================
  const fetchProfile = async (user) => {
    if (!user) {
      setProfile(null);
      setProfileLoading(false);
      return;
    }

    setProfileLoading(true);
    setTimeout(() => setProfileLoading(false), 5000); // Fallback to prevent infinite loading

    const { data, error } = await supabase
      .from("profiles")
      .select("*")
      .eq("id", user.id)
      .single();

    if (error) {
      console.error("Profile fetch error:", error.message);
      setProfile(null);
    } else {
      setProfile(data);
    }

    setProfileLoading(false);
  };

  // ======================
  // INIT SESSION (ON LOAD)
  // ======================
  useEffect(() => {
    const init = async () => {
      const { data } = await supabase.auth.getSession();

      const session = data.session;
      const user = session?.user ?? null;

      setSession(session);
      setUser(user);

      setAuthLoading(false); // auth is done

      await fetchProfile(user);
    };

    init();

    // ======================
    // AUTH LISTENER
    // ======================
    const { data: listener } = supabase.auth.onAuthStateChange(
      async (_event, session) => {
        const user = session?.user ?? null;

        setSession(session);
        setUser(user);

        setAuthLoading(false);

        await fetchProfile(user);
      }
    );

    return () => listener.subscription.unsubscribe();
  }, []);

  // ======================
  // SIGN OUT
  // ======================
  const signOut = async () => {
    await supabase.auth.signOut();

    setSession(null);
    setUser(null);
    setProfile(null);

    setProfileLoading(false);
  };

  // ======================
  // FINAL LOADING STATE
  // ======================
  const loading = authLoading || profileLoading;

  // ======================
  // SAFE ROLE HELPER
  // ======================
  const role = profile?.role ?? "user";

  // ======================
  // CONTEXT VALUE
  // ======================
  const value = {
    session,
    user,
    profile,
    role,

    loading,
    authLoading,
    profileLoading,

    signOut,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);