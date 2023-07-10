import { Box, IconButton, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { observer } from "mobx-react";
import { CandsSourceEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { MdOutlineEdit } from "react-icons/md";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
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
      <Box>
        {candsStore.currentTabCandsLists !== TabsCandsEnum.None && (
          <Tabs
            value={candsStore.currentTabCandsLists}
            variant="scrollable"
            scrollButtons
            allowScrollButtonsMobile
            onChange={handleTabChange}
          >
            {foldersStore.selectedFolder?.id && (
              <Tab
                label={foldersStore.selectedFolder?.name}
                value={TabsCandsEnum.FolderCands}
                sx={{
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  alignSelf: "flex-start",
                }}
              />
            )}
            {positionsStore.selectedPosition?.name && (
              <Tab
                label={
                  <div>
                    <div>{positionsStore.selectedPosition?.name}</div>
                    <div>{positionsStore.selectedPosition?.customerName}</div>
                  </div>
                }
                value={TabsCandsEnum.PositionCands}
                sx={{
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  alignSelf: "flex-start",
                }}
              />
            )}
            <Tab
              label="Candidates"
              value={TabsCandsEnum.AllCands}
              sx={{
                alignSelf: "flex-start",
              }}
            />
          </Tabs>
        )}
      </Box>
      {/* <CacheProvider value={cacheRtl}>
        <ThemeProvider theme={themeRtl}> */}
      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands}>
        <Box mt={1} mr={2}>
          <SearchControl onSearch={handleAllCandsSearch} />
        </Box>
        <CandsList
          candsListData={candsStore.candsAllList}
          candsSource={CandsSourceEnum.AllCands}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
        <Box mt={1} mr={2}>
          <Stack direction="row" gap={1}>
            <SearchControl onSearch={handlePositionCandsSearch} />
          </Stack>
        </Box>
        <CandsList
          candsListData={candsStore.posCandsList}
          candsSource={CandsSourceEnum.Position}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <Box mt={1} mr={2}>
          <SearchControl onSearch={handleFolderCandsSearch} />
        </Box>
        <CandsList
          candsListData={candsStore.folderCandsList}
          candsSource={CandsSourceEnum.Folder}
        />
      </div>
      {/* </ThemeProvider>
      </CacheProvider> */}
    </Box>
  );
});
