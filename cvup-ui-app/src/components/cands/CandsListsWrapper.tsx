import { Box, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { observer } from "mobx-react";
import { CandsSourceEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { ICand, ISearchModel } from "../../models/GeneralModels";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
  // const [folderId, setFolderId] = useState(0);
  const [candsPosList, setCandsPosList] = useState<ICand[]>([]);
  const [candsFolderList, setCandsFolderList] = useState<ICand[]>([]);
  const [candsAllList, setCandsAllList] = useState<ICand[]>([]);
  const [candsAdvancedOpen, setCandsAdvancedOpen] = useState(false);
  const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);
  const [foldersAdvancedOpen, setFoldersAdvancedOpen] = useState(false);

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

  const handleAllCandsSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.searchAllCands(searchVals);
    } else {
      candsStore.getCandsList();
    }
  };

  const handlePositionCandsSearch = (searchVals: ISearchModel) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.PositionCands) {
      if (searchVals.value) {
        candsStore.searchPositionCands(searchVals);
      } else {
        candsStore.getPositionCands();
      }
    }
  };

  const handleFolderCandsSearch = (searchVals: ISearchModel) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.FolderCands) {
      if (searchVals.value) {
        candsStore.searchFolderCands(searchVals);
      } else {
        candsStore.getFolderCandsList();
      }
    }
  };

  const handleShowAdvanced = (val: boolean) => {};

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
        <Box mt={1} mr={1} ml={1}>
          <SearchControl
            onSearch={handleAllCandsSearch}
            onShowAdvanced={() => setCandsAdvancedOpen(!candsAdvancedOpen)}
            shoeAdvancedIcon={true}
          />
        </Box>
        <CandsList
          candsListData={candsAllList}
          candsSource={CandsSourceEnum.AllCands}
          advancedOpen={candsAdvancedOpen}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
        <Box mt={1} mr={2}>
          <SearchControl
            onSearch={handlePositionCandsSearch}
            onShowAdvanced={() =>
              setPositionsAdvancedOpen(!positionsAdvancedOpen)
            }
            shoeAdvancedIcon={true}
          />
        </Box>
        <CandsList
          candsListData={candsPosList}
          candsSource={CandsSourceEnum.Position}
          advancedOpen={positionsAdvancedOpen}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <Box mt={1} mr={2}>
          <SearchControl
            onSearch={handleFolderCandsSearch}
            onShowAdvanced={() => setFoldersAdvancedOpen(!foldersAdvancedOpen)}
            shoeAdvancedIcon={true}
          />
        </Box>
        <CandsList
          candsListData={candsFolderList}
          candsSource={CandsSourceEnum.Folder}
          advancedOpen={foldersAdvancedOpen}
        />
      </div>
    </Box>
  );
});
