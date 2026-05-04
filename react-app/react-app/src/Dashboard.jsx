import { useState } from "react";
import { useAuth } from "./context/AuthContext";
import {
  AppBar,
  Toolbar,
  Typography,
  Container,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Chip,
  Grid,
  Stack,
} from "@mui/material";

export default function Dashboard() {
  const { user, profile, signOut, loading } = useAuth();
  const [loggingOut, setLoggingOut] = useState(false);

  // 🧠 SAFE ROLE (single source of truth)
  const role = profile?.role ?? "user";

  const roleColor =
    role === "superadmin"
      ? "error"
      : role === "admin"
      ? "warning"
      : "primary";

  // 🚪 Logout handler with UX state
  const handleLogout = async () => {
    setLoggingOut(true);
    await signOut();
    setLoggingOut(false);
  };

  return (
    <>
      {/* NAVBAR */}
      <AppBar position="static" elevation={2}>
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            My App Dashboard
          </Typography>

          <Button
            color="inherit"
            onClick={handleLogout}
            disabled={loggingOut}
          >
            {loggingOut ? "Logging out..." : "Logout"}
          </Button>
        </Toolbar>
      </AppBar>

      {/* MAIN */}
      <Container sx={{ mt: 4 }}>

        {/* HEADER */}
        <Box sx={{ mb: 3 }}>
          <Typography variant="h4" fontWeight="bold">
            Welcome 👋
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Here is your account overview
          </Typography>
        </Box>

        {/* STATS */}
        <Grid container spacing={3} sx={{ mb: 4 }}>
          
          {/* Account Status */}
          <Grid item xs={12} md={4}>
            <Card sx={{ p: 1, boxShadow: 3 }}>
              <CardContent>
                <Typography variant="subtitle1" fontWeight="bold">
                  Account Status
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Active session
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          {/* Role */}
          <Grid item xs={12} md={4}>
            <Card sx={{ p: 1, boxShadow: 3 }}>
              <CardContent>
                <Typography variant="subtitle1" fontWeight="bold">
                  Role
                </Typography>

                <Chip
                  label={role}
                  color={roleColor}
                  sx={{ mt: 1, fontWeight: "bold" }}
                />
              </CardContent>
            </Card>
          </Grid>

          {/* Membership */}
          <Grid item xs={12} md={4}>
            <Card sx={{ p: 1, boxShadow: 3 }}>
              <CardContent>
                <Typography variant="subtitle1" fontWeight="bold">
                  Membership
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Not assigned yet
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* PROFILE SECTION */}
        <Card sx={{ maxWidth: 700, mx: "auto", boxShadow: 6, borderRadius: 3 }}>
          <CardContent>
            <Typography variant="h6" fontWeight="bold" gutterBottom>
              Account Details
            </Typography>

            <Stack spacing={2}>
              
              {/* Email */}
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Email
                </Typography>
                <Typography variant="body1">
                  {user?.email}
                </Typography>
              </Box>

              {/* User ID */}
              <Box>
                <Typography variant="body2" color="text.secondary">
                  User ID
                </Typography>
                <Typography
                  variant="body1"
                  sx={{ wordBreak: "break-word" }}
                >
                  {user?.id}
                </Typography>
              </Box>

              {/* Role */}
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Role
                </Typography>

                <Chip
                  label={role}
                  color={roleColor}
                  sx={{ mt: 1 }}
                />
              </Box>

            </Stack>
          </CardContent>
        </Card>
      </Container>
    </>
  );
}