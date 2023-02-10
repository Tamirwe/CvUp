import { Box, Grid, Paper, Tab, Tabs } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { observer } from "mobx-react";
import { PosCandsList } from "./PosCandsList";

export const CandsListsWrapper = observer(() => {
  const { cvsStore, positionsStore } = useStore();

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

  const handleTabChange = (event: React.SyntheticEvent, newValue: string) => {
    cvsStore.currentTabCandsList = newValue;
  };

  return (
    <Box sx={{ marginTop: "0" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={cvsStore.currentTabCandsList}
          onChange={handleTabChange}
          sx={{ direction: "rtl" }}
        >
          <Tab label="Candidates" value="candList" />
          <Tab
            label={positionsStore.posSelected?.name}
            value="positionCandsList"
            sx={{
              overflow: "hidden",
              whiteSpace: "nowrap",
              textOverflow: "ellipsis",
            }}
          />
        </Tabs>
      </Box>
      <CacheProvider value={cacheRtl}>
        <ThemeProvider theme={themeRtl}>
          {cvsStore.currentTabCandsList === "candList" ? (
            <CandsList candsList={cvsStore.candsList} />
          ) : (
            <CandsList candsList={cvsStore.posCandsList} />
          )}
        </ThemeProvider>
      </CacheProvider>
    </Box>
  );
});
