import { useState } from 'react'
import { supabase } from '../lib/supabase'
import { useNavigate, Link } from 'react-router-dom'

export default function Login() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const nav = useNavigate()

  const login = async () => {
    const { data, error } = await supabase.auth.signInWithPassword({
      email,
      password
    })

    if (data.user) nav('/')
  }

  return (
    <div>
      <h2>Login</h2>
      <input onChange={e => setEmail(e.target.value)} placeholder="email" />
      <input onChange={e => setPassword(e.target.value)} type="password" />
      <button onClick={login}>Login</button>
      <p>Don't have an account? <Link to="/register">Register</Link></p>
    </div>
  )
}