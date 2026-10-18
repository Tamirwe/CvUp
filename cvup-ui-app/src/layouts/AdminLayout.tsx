import { Box, Button, Stack, Typography } from "@mui/material";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { AppLogo } from "../components/header/AppLogo";

// The ground the admin pages sit on. A soft grey-blue instead of white so the
// panels read as cards and the screen does not glare; the page's own card
// colours are tuned against this value.
const SURFACE = "#eceff3";
const BAR_BG = "#f7f9fa";
const BAR_BORDER = "#dfe4ea";
const INK_SECONDARY = "#5d6b77";
const ACCENT = "#4d7c9e";

// Every admin module, in the order they appear in the bar. Add to this and the
// nav picks it up -- a module is reachable from here, not only from whatever
// widget happens to link to it. The dashboard is not in the list: the "Admin"
// label itself goes there.
const ADMIN_PAGES = [{ label: "Customers", path: "/admin/customers" }];
const DASHBOARD_PATH = "/admin";

export const AdminLayout = observer(() => {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <Box sx={{ flexGrow: 1, backgroundColor: SURFACE, minHeight: "100vh" }}>
      {/* A bar rather than a floating logo: it gives the transparent mark a
          surface of its own and separates it from the page below. */}
      <Stack
        direction="row"
        alignItems="center"
        spacing={isMobile ? 1 : 1.5}
        sx={{
          height: "3.5rem",
          padding: isMobile ? "0 0.5rem" : "0 1rem",
          backgroundColor: BAR_BG,
          borderBottom: `1px solid ${BAR_BORDER}`,
        }}
      >
        <AppLogo
          height={isMobile ? 24 : 30}
          title="Back to the main app"
          onClick={() => navigate("/")}
        />
        <Box
          sx={{ width: "1px", height: "1.4rem", backgroundColor: BAR_BORDER }}
        />
        {/* Shown on phones too, now that it is the way back to the dashboard
            rather than a label. */}
        <Typography
          onClick={() => navigate(DASHBOARD_PATH)}
          sx={{
            fontSize: "0.8rem",
            textTransform: "uppercase",
            letterSpacing: "0.08em",
            cursor: "pointer",
            fontWeight: location.pathname === DASHBOARD_PATH ? 700 : 500,
            color:
              location.pathname === DASHBOARD_PATH ? ACCENT : INK_SECONDARY,
            "&:hover": { color: ACCENT },
          }}
        >
          Admin
        </Typography>

        <Stack direction="row" spacing={0.5}>
          {ADMIN_PAGES.map((page) => {
            const isCurrent = location.pathname === page.path;

            return (
              <Button
                key={page.path}
                size="small"
                onClick={() => navigate(page.path)}
                sx={{
                  textTransform: "none",
                  fontSize: "0.85rem",
                  fontWeight: isCurrent ? 700 : 500,
                  color: isCurrent ? ACCENT : INK_SECONDARY,
                  // The current page keeps an underline rather than a filled
                  // background, so the bar stays quiet.
                  borderBottom: `2px solid ${isCurrent ? ACCENT : "transparent"}`,
                  borderRadius: 0,
                }}
              >
                {page.label}
              </Button>
            );
          })}
        </Stack>
      </Stack>
      <Outlet />
    </Box>
  );
});
