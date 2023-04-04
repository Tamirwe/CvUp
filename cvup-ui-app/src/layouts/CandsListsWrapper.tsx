import { Box, IconButton, Stack, Tab, Tabs } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../Hooks/useStore";
import { CandsList } from "../components/cands/CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { observer } from "mobx-react";
import { TabsCandsEnum } from "../models/GeneralEnums";
import { SearchCands } from "../components/header/SearchCands";
import { SearchControl } from "../components/header/SearchControl";
import { MdOutlineEdit } from "react-icons/md";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore, generalStore } = useStore();

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
    candsStore.currentTabCandsLists = newValue;
  };

  return (
    <Box sx={{ marginTop: "0" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={candsStore.currentTabCandsLists}
          onChange={handleTabChange}
          sx={{ direction: "rtl" }}
        >
          <Tab label="Candidates" value={TabsCandsEnum.AllCands} />
          <Tab
            label={positionsStore.selectedPosition?.name}
            value={TabsCandsEnum.PositionCands}
            sx={{
              overflow: "hidden",
              whiteSpace: "nowrap",
              textOverflow: "ellipsis",
            }}
          />
          <Tab
            label={foldersStore.selectedFolder?.name}
            value={TabsCandsEnum.FolderCands}
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
          {candsStore.currentTabCandsLists === TabsCandsEnum.AllCands && (
            <>
              <Box mt={1} ml={1}>
                <SearchControl />
              </Box>
              <CandsList candsListData={candsStore.candsAllList} />
            </>
          )}
          {candsStore.currentTabCandsLists === TabsCandsEnum.PositionCands && (
            <>
              <Box mt={1} ml={1}>
                <Stack direction="row" gap={1}>
                  <IconButton
                    title="Edit position"
                    size="medium"
                    onClick={async () => {
                      await positionsStore.getPosition(
                        positionsStore.selectedPosition?.id || 0
                      );
                      generalStore.showPositionFormDialog = true;
                    }}
                  >
                    <MdOutlineEdit />
                  </IconButton>
                  <SearchControl />
                </Stack>
              </Box>
              <CandsList candsListData={candsStore.posCandsList} />
            </>
          )}
          {candsStore.currentTabCandsLists === TabsCandsEnum.FolderCands && (
            <>
              <Box mt={1} ml={1}>
                <SearchControl />
              </Box>
              <CandsList candsListData={candsStore.folderCandsList} />
            </>
          )}
        </ThemeProvider>
      </CacheProvider>
    </Box>
  );
});
