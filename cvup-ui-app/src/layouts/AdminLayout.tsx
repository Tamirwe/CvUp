import { Box, Stack, Typography } from "@mui/material";
import { Outlet, useNavigate } from "react-router-dom";
import { observer } from "mobx-react";
import { AppLogo } from "../components/header/AppLogo";

// The ground the admin pages sit on. A soft grey-blue instead of white so the
// panels read as cards and the screen does not glare; the page's own card
// colours are tuned against this value.
const SURFACE = "#eceff3";
const BAR_BG = "#f7f9fa";
const BAR_BORDER = "#dfe4ea";

// Otherwise deliberately undesigned: the only thing wired up here is the logo,
// which is the way back to the main (recruiter) layout -- the same gesture that
// got the user here.
export const AdminLayout = observer(() => {
  const navigate = useNavigate();

  return (
    <Box sx={{ flexGrow: 1, backgroundColor: SURFACE, minHeight: "100vh" }}>
      {/* A bar rather than a floating logo: it gives the transparent mark a
          surface of its own and separates it from the page below. */}
      <Stack
        direction="row"
        alignItems="center"
        spacing={1.5}
        sx={{
          height: "3.5rem",
          padding: "0 1rem",
          backgroundColor: BAR_BG,
          borderBottom: `1px solid ${BAR_BORDER}`,
        }}
      >
        <AppLogo
          height={30}
          title="Back to the main app"
          onClick={() => navigate("/")}
        />
        <Box
          sx={{ width: "1px", height: "1.4rem", backgroundColor: BAR_BORDER }}
        />
        <Typography
          sx={{
            fontSize: "0.8rem",
            textTransform: "uppercase",
            letterSpacing: "0.08em",
          }}
          color="#5d6b77"
        >
          Admin
        </Typography>
      </Stack>
      <Outlet />
    </Box>
  );
});
