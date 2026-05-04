import { useState, useEffect } from "react"
import { supabase } from "../lib/supabase"

export default function Admin() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [role, setRole] = useState("user")
  const [users, setUsers] = useState([])

  const FUNCTION_URL = import.meta.env.VITE_FUNCTION_URL

  // 🔐 GET SESSION TOKEN
  const getToken = async () => {
    const { data } = await supabase.auth.getSession()
    return data.session?.access_token
  }

  // 📌 CREATE USER
  const createUser = async () => {
    const token = await getToken()

    const res = await fetch(FUNCTION_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify({
        email,
        password,
        role,
        full_name: email.split("@")[0],
      }),
    })

    const data = await res.json()
    console.log("CREATE USER RESPONSE:", data)

    if (!res.ok) {
      alert("Error: " + JSON.stringify(data))
      return
    }

    alert("User created successfully!")
    fetchUsers()
  }

  // 📌 FETCH USERS
  const fetchUsers = async () => {
    const { data, error } = await supabase
      .from("profiles")
      .select("*")
      .order("created_at", { ascending: false })

    if (!error) setUsers(data)
  }

  useEffect(() => {
    fetchUsers()
  }, [])

  return (
    <div style={{ padding: 20 }}>
      <h2>Admin Panel</h2>

      {/* CREATE USER */}
      <div>
        <h3>Create User</h3>

        <input
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <br />

        <input
          placeholder="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <br />

        <select value={role} onChange={(e) => setRole(e.target.value)}>
          <option value="user">User</option>
          <option value="admin">Admin</option>
        </select>

        <br />

        <button onClick={createUser}>Create User</button>
      </div>

      {/* USERS */}
      <h3>Users</h3>
      <ul>
        {users?.map((u) => (
          <li key={u.id}>
            {u.full_name} — {u.role}
          </li>
        ))}
      </ul>
    </div>
  )
}