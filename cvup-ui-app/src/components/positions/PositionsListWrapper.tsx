import { Box, Button } from "@mui/material";
import { PositionsList } from "./PositionsList";
import { GoPlus } from "react-icons/go";
import { useNavigate } from "react-router-dom";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";

export const PositionsListWrapper = () => {
  const navigate = useNavigate();

  const themeRtl = createTheme({
    direction: "rtl", // Both here and <body dir="rtl">
  });

  // Create rtl cache
  const cacheRtl = createCache({
    key: "muirtl",
    stylisPlugins: [rtlPlugin],
  });

  return (
    <Box>
      <Box>
        <Button
          sx={{ width: "fit-content" }}
          onClick={() => navigate("/position/0")}
          startIcon={<GoPlus />}
        >
          Add Position
        </Button>
      </Box>
      <Box
        sx={{
          height: "83vh",
          // display: "flex",
          // flexDirection: "column",
          // flexWrap: "wrap",
          // "& > :not(style)": {
          //   m: 1,
          // },
        }}
      >
        {/* <Paper elevation={3}> */}
        <CacheProvider value={cacheRtl}>
          <ThemeProvider theme={themeRtl}>
            <PositionsList />
          </ThemeProvider>
        </CacheProvider>

        {/* </Paper> */}
      </Box>
      <Box>
        <Button
          sx={{ width: "fit-content" }}
          onClick={() => navigate("/position/0")}
          startIcon={<GoPlus />}
        >
          Add Position
        </Button>
      </Box>
    </Box>
  );
};
