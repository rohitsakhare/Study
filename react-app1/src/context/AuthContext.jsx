import { createContext, useContext, useEffect, useState } from 'react'
import { supabase } from '../lib/supabase'

const AuthContext = createContext()

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)

  const fetchProfile = async (userId) => {
    let { data, error } = await supabase
      .from('profiles')
      .select('*')
      .eq('id', userId)
      .single()

    if (!data) {
      // Create default profile if not exists
      const { data: newProfile } = await supabase
        .from('profiles')
        .insert({ id: userId, role: 'user' })
        .select()
        .single()
      data = newProfile
    }

    setProfile(data)
  }

  useEffect(() => {
    supabase.auth.getSession().then(({ data }) => {
      const user = data.session?.user
      setUser(user)

      if (user) fetchProfile(user.id)
      setLoading(false)
    })

    const { data: listener } = supabase.auth.onAuthStateChange(
      (_, session) => {
        const user = session?.user
        setUser(user)
        if (user) fetchProfile(user.id)
      }
    )

    return () => listener.subscription.unsubscribe()
  }, [])
  console.log("USER:", user)
  console.log("PROFILE:", profile)
  return (
    <AuthContext.Provider value={{ user, profile, loading }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)