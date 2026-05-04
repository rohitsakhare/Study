import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

export default function Register() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fullName, setFullName] = useState('')
  const [role, setRole] = useState('user')
  const nav = useNavigate()

  const register = async () => {
    const response = await fetch(import.meta.env.VITE_FUNCTION_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email,
        password,
        full_name: fullName,
        role,
      }),
    })

    if (response.ok) {
      alert('User created successfully! Please log in.')
      nav('/login')
    } else {
      alert('Error creating user')
    }
  }

  return (
    <div>
      <h2>Register</h2>
      <input onChange={e => setFullName(e.target.value)} placeholder="Full Name" />
      <input onChange={e => setEmail(e.target.value)} placeholder="email" />
      <input onChange={e => setPassword(e.target.value)} type="password" placeholder="password" />
      <select onChange={e => setRole(e.target.value)} value={role}>
        <option value="user">User</option>
        <option value="admin">Admin</option>
        <option value="superadmin">Super Admin</option>
      </select>
      <button onClick={register}>Register</button>
    </div>
  )
}