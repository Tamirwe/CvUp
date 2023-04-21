import { Box, IconButton, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../Hooks/useStore";
import { CandsList } from "../components/cands/CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { observer } from "mobx-react";
import { TabsCandsEnum } from "../models/GeneralEnums";
import { SearchControl } from "../components/header/SearchControl";
import { MdOutlineEdit } from "react-icons/md";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore, generalStore } = useStore();
  const [folderId, setFolderId] = useState(0);

  useEffect(() => {
    if (foldersStore.selectedFolder?.id) {
      setFolderId(foldersStore.selectedFolder.id);
    }
  }, [foldersStore.selectedFolder?.id]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    candsStore.currentTabCandsLists = TabsCandsEnum.AllCands;
    candsStore.getCandsList();
    // if (
    //   candsStore.currentTabCandsLists === TabsCandsEnum.AllCands &&
    //   candsStore.candsAllList.length === 0
    // ) {
    //   candsStore.getCandsList();
    // }
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

  const handleAllCandsSearch = (val: string) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.AllCands) {
      if (val) {
        candsStore.searchAllCands(val);
      } else {
        candsStore.getCandsList();
      }
    }
  };

  const handlePositionCandsSearch = (val: string) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.PositionCands) {
      if (val) {
        candsStore.searchPositionCands(val);
      } else {
        candsStore.getPositionCands();
      }
    }
  };

  const handleFolderCandsSearch = (val: string) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.FolderCands) {
      if (val) {
        candsStore.searchFolderCands(val);
      } else {
        candsStore.getFolderCandsList();
      }
    }
  };

  return (
    <Box sx={{ marginTop: "0" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        {candsStore.currentTabCandsLists !== TabsCandsEnum.None && (
          <Tabs
            value={candsStore.currentTabCandsLists}
            onChange={handleTabChange}
            sx={{ direction: "rtl" }}
          >
            <Tab label="Candidates" value={TabsCandsEnum.AllCands} />
            {positionsStore.selectedPosition?.name && (
              <Tab
                label={positionsStore.selectedPosition?.name}
                value={TabsCandsEnum.PositionCands}
                sx={{
                  overflow: "hidden",
                  whiteSpace: "nowrap",
                  textOverflow: "ellipsis",
                }}
              />
            )}
            {foldersStore.selectedFolder?.id && (
              <Tab
                label={foldersStore.selectedFolder?.name}
                value={TabsCandsEnum.FolderCands}
                sx={{
                  overflow: "hidden",
                  whiteSpace: "nowrap",
                  textOverflow: "ellipsis",
                }}
              />
            )}
          </Tabs>
        )}
      </Box>
      {/* <CacheProvider value={cacheRtl}>
        <ThemeProvider theme={themeRtl}> */}
      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands}>
        <Box mt={1} ml={1}>
          <SearchControl onSearch={handleAllCandsSearch} />
        </Box>
        <CandsList candsListData={candsStore.candsAllList} />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
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
            <SearchControl onSearch={handlePositionCandsSearch} />
          </Stack>
        </Box>
        <CandsList candsListData={candsStore.posCandsList} />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <Box mt={1} ml={1}>
          <SearchControl onSearch={handleFolderCandsSearch} />
        </Box>
        <CandsList
          candsListData={candsStore.folderCandsList}
          // candsListData={
          //   foldersStore.selectedFolder?.id === folderId
          //     ? candsStore.folderCandsList
          //     : []
          // }
        />
      </div>
      {/* </ThemeProvider>
      </CacheProvider> */}
    </Box>
  );
});
