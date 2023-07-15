import { Box, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { observer } from "mobx-react";
import { CandsSourceEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { ICand } from "../../models/GeneralModels";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
  // const [folderId, setFolderId] = useState(0);
  const [candsPosList, setCandsPosList] = useState<ICand[]>([]);
  const [candsFolderList, setCandsFolderList] = useState<ICand[]>([]);
  const [candsAllList, setCandsAllList] = useState<ICand[]>([]);

  // useEffect(() => {
  //   if (foldersStore.selectedFolder?.id) {
  //     setFolderId(foldersStore.selectedFolder.id);
  //   }
  // }, [foldersStore.selectedFolder?.id]); // eslint-disable-line react-hooks/exhaustive-deps

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

  useEffect(() => {
    setCandsAllList(candsStore.candsAllList);
  }, [candsStore.candsAllList]);

  useEffect(() => {
    setCandsPosList(candsStore.posCandsList);
  }, [candsStore.posCandsList]);

  useEffect(() => {
    setCandsFolderList(candsStore.folderCandsList);
  }, [candsStore.folderCandsList]);

  // const themeRtl = createTheme({
  //   direction: "rtl", // Both here and <body dir="rtl">
  // });

  // // Create rtl cache
  // const cacheRtl = createCache({
  //   key: "muirtl",
  //   stylisPlugins: [rtlPlugin],
  // });

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
    <Box sx={{ marginTop: "0", backgroundColor: "white" }}>
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

      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands}>
        <Box mt={1} mr={2}>
          <SearchControl onSearch={handleAllCandsSearch} />
        </Box>
        <CandsList
          candsListData={candsAllList}
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
          candsListData={candsPosList}
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
          candsListData={candsFolderList}
          candsSource={CandsSourceEnum.Folder}
        />
      </div>
    </Box>
  );
});
