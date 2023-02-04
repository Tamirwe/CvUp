import { Box } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";

export const CandsListWrapper = () => {
  const { cvsStore } = useStore();

  useEffect(() => {
    cvsStore.getCandsList();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const themeRtl = createTheme({
    direction: "rtl", // Both here and <body dir="rtl">
  });

  // Create rtl cache
  const cacheRtl = createCache({
    key: "muirtl",
    stylisPlugins: [rtlPlugin],
  });

  return (
    <Box sx={{ marginTop: "0" }}>
      <CacheProvider value={cacheRtl}>
        <ThemeProvider theme={themeRtl}>
          <CandsList />
        </ThemeProvider>
      </CacheProvider>
    </Box>
  );
};
