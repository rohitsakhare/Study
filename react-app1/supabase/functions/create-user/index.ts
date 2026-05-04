import { createClient } from "npm:@supabase/supabase-js"

const corsHeaders = {
  "Access-Control-Allow-Origin": "*",
  "Access-Control-Allow-Headers": "authorization, x-client-info, apikey, content-type",
  "Access-Control-Allow-Methods": "POST, OPTIONS",
}

const supabase = createClient(
  Deno.env.get("SUPABASE_URL")!,
  Deno.env.get("SUPABASE_SERVICE_ROLE_KEY")!
)

Deno.serve(async (req) => {
  // ✅ CORS preflight
  if (req.method === "OPTIONS") {
    return new Response("ok", { headers: corsHeaders })
  }

  try {
    // 🔐 AUTH CHECK
    const authHeader = req.headers.get("Authorization")
    if (!authHeader) {
      return new Response(
        JSON.stringify({ error: "Missing auth header" }),
        { status: 401, headers: corsHeaders }
      )
    }

    const token = authHeader.replace("Bearer ", "")

    const { data: userData } = await supabase.auth.getUser(token)

    if (!userData?.user) {
      return new Response(
        JSON.stringify({ error: "Invalid user" }),
        { status: 401, headers: corsHeaders }
      )
    }

    // 📥 BODY
    const { email, password, role, full_name } = await req.json()

    // 🔥 CREATE AUTH USER
    const { data, error } = await supabase.auth.admin.createUser({
      email,
      password,
      email_confirm: true,
    })

    if (error) {
      return new Response(JSON.stringify(error), {
        status: 400,
        headers: corsHeaders,
      })
    }

    // 📌 INSERT PROFILE
    const { error: profileError } = await supabase
      .from("profiles")
      .insert({
        id: data.user.id,
        full_name,
        role,
      })

    if (profileError) {
      return new Response(JSON.stringify(profileError), {
        status: 400,
        headers: corsHeaders,
      })
    }

    return new Response(
      JSON.stringify({ success: true, user: data.user }),
      { headers: corsHeaders }
    )
  } catch (err) {
    return new Response(
      JSON.stringify({ error: err.message }),
      { status: 500, headers: corsHeaders }
    )
  }
})