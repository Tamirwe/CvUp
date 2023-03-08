import { Box, Tab, Tabs } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../Hooks/useStore";
import { CandsList } from "../components/cands/CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { observer } from "mobx-react";
import { TabsCandsEnum } from "../models/GeneralEnums";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();

  useEffect(() => {
    candsStore.getCandsList();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const themeRtl = createTheme({
    direction: "rtl", // Both here and <body dir="rtl">
  });

  // Create rtl cache
  const cacheRtl = createCache({
    key: "muirtl",
    stylisPlugins: [rtlPlugin],
  });

  const handleTabChange = (
    event: React.SyntheticEvent,
    newValue: TabsCandsEnum
  ) => {
    candsStore.currentTabCandsList = newValue;
  };

  return (
    <Box sx={{ marginTop: "0" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={candsStore.currentTabCandsList}
          onChange={handleTabChange}
          sx={{ direction: "rtl" }}
        >
          <Tab label="Candidates" value="candList" />
          <Tab
            label={positionsStore.selectedPosition?.name}
            value="positionCandsList"
            sx={{
              overflow: "hidden",
              whiteSpace: "nowrap",
              textOverflow: "ellipsis",
            }}
          />
          <Tab
            label={foldersStore.selectedFolder?.name}
            value="folderCandsList"
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
          {candsStore.currentTabCandsList === TabsCandsEnum.AllCands ? (
            <CandsList candsListData={candsStore.candsList} />
          ) : (
            <CandsList candsListData={candsStore.posCandsList} />
          )}
        </ThemeProvider>
      </CacheProvider>
    </Box>
  );
});
