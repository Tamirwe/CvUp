import { Box, Paper, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "../cands/CandsList";
import { observer } from "mobx-react";
import {
  CandsSourceEnum,
  SortByEnum,
  TabsCandsEnum,
} from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { ICand, ISearchModel } from "../../models/GeneralModels";
import { isMobile } from "react-device-detect";

export const CandsListsContainer = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
  // const [folderId, setFolderId] = useState(0);
  const [allCandsList, setAllCandsList] = useState<ICand[]>([]);
  const [candsPosList, setCandsPosList] = useState<ICand[]>([]);
  const [candsPosTypeList, setCandsPosTypeList] = useState<ICand[]>([]);
  const [candsFolderList, setCandsFolderList] = useState<ICand[]>([]);
  const [candsAdvancedOpen, setCandsAdvancedOpen] = useState(false);
  const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);
  const [positionsTypesAdvancedOpen, setPositionsTypesAdvancedOpen] =
    useState(false);
  const [foldersAdvancedOpen, setFoldersAdvancedOpen] = useState(false);

  const sortCandList = (sortBy: SortByEnum, dir: string, list: ICand[]) => {
    if (sortBy === SortByEnum.cvDate) {
      const sorted = list.slice();

      if (dir === "asc") {
        return sorted.sort((a, b) =>
          a.cvSent > b.cvSent ? 1 : b.cvSent > a.cvSent ? -1 : 0
        );
      }

      return sorted.sort((a, b) =>
        a.cvSent < b.cvSent ? 1 : b.cvSent < a.cvSent ? -1 : 0
      );
    } else {
      const sorted = list.slice();

      if (dir === "asc") {
        return sorted.sort((a, b) =>
          a.score > b.score ? 1 : b.score > a.score ? -1 : 0
        );
      }

      return sorted.sort((a, b) =>
        a.score < b.score ? 1 : b.score < a.score ? -1 : 0
      );
    }
  };

  useEffect(() => {
    setAllCandsList(candsStore.allCandsList);
  }, [candsStore.allCandsList]);

  useEffect(() => {
    setCandsPosList(candsStore.posCandsList);
  }, [candsStore.posCandsList]);

  useEffect(() => {
    setCandsPosTypeList(candsStore.posTypeCandsList);
  }, [candsStore.posTypeCandsList]);

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
    event.stopPropagation();
    event.preventDefault();
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
        if (positionsStore.selectedPosition?.id) {
          candsStore.getPositionCandsList(positionsStore.selectedPosition?.id);
          candsStore.setDisplayCandOntopPCList();
        }
      }
    }
  };

  const handlePositionTypeCandsSearch = (searchVals: ISearchModel) => {
    if (candsStore.currentTabCandsLists === TabsCandsEnum.PositionTypeCands) {
      if (searchVals.value) {
        candsStore.searchPositionTypeCands(searchVals);
      } else {
        if (positionsStore.selectedPositionType?.id) {
          candsStore.getPositionTypeCandsList(
            positionsStore.selectedPositionType?.id
          );
        }
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

  const handleSort = (
    sortBy: SortByEnum,
    dir: string,
    ListSource: TabsCandsEnum
  ) => {
    switch (ListSource) {
      case TabsCandsEnum.AllCands:
        setAllCandsList(sortCandList(sortBy, dir, candsStore.allCandsList));
        break;
      case TabsCandsEnum.PositionCands:
        setCandsPosList(sortCandList(sortBy, dir, candsStore.posCandsList));
        break;
      case TabsCandsEnum.FolderCands:
        setCandsFolderList(
          sortCandList(sortBy, dir, candsStore.folderCandsList)
        );
        break;
      default:
        break;
    }
  };

  return (
    <Paper
      elevation={3}
      sx={{ margin: isMobile ? "0" : "0.7rem 0.7rem 0 0.7rem" }}
    >
      <Box>
        {candsStore.currentTabCandsLists !== TabsCandsEnum.None && (
          <Tabs
            sx={{
              "&.MuiTabs-root": { height: "3.7rem" },
              direction: "ltr",
            }}
            value={candsStore.currentTabCandsLists}
            variant="scrollable"
            scrollButtons
            allowScrollButtonsMobile
            onChange={handleTabChange}
            onClick={(event) => {
              event.stopPropagation();
              event.preventDefault();
            }}
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
            {positionsStore.selectedPositionType?.typeName && (
              <Tab
                label={
                  <div>
                    <div>
                      {positionsStore.selectedPositionType?.typeName.substring(
                        0,
                        20
                      )}
                    </div>
                    <div>PT</div>
                  </div>
                }
                value={TabsCandsEnum.PositionTypeCands}
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
            records={candsStore.allCandsList && candsStore.allCandsList.length}
            onSort={(sortBy: SortByEnum, dir: string) => {
              handleSort(sortBy, dir, TabsCandsEnum.AllCands);
            }}
            showRefreshList={true}
          />
        </Box>
        <CandsList
          candsListData={allCandsList}
          candsSource={CandsSourceEnum.AllCands}
          advancedOpen={candsAdvancedOpen}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
        <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden" }}>
          <SearchControl
            onSearch={handlePositionCandsSearch}
            onShowAdvanced={() =>
              setPositionsAdvancedOpen(!positionsAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={candsStore.posCandsList && candsStore.posCandsList.length}
            onSort={(sortBy: SortByEnum, dir: string) => {
              handleSort(sortBy, dir, TabsCandsEnum.PositionCands);
            }}
            showRefreshList={true}
          />
        </Box>
        <CandsList
          candsListData={candsPosList}
          candsSource={CandsSourceEnum.Position}
          advancedOpen={positionsAdvancedOpen}
        />
      </div>
      <div
        hidden={
          candsStore.currentTabCandsLists !== TabsCandsEnum.PositionTypeCands
        }
      >
        <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden" }}>
          <SearchControl
            onSearch={handlePositionTypeCandsSearch}
            onShowAdvanced={() =>
              setPositionsTypesAdvancedOpen(!positionsTypesAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={
              candsStore.posTypeCandsList && candsStore.posTypeCandsList.length
            }
            onSort={(sortBy: SortByEnum, dir: string) => {
              handleSort(sortBy, dir, TabsCandsEnum.PositionTypeCands);
            }}
            showRefreshList={true}
          />
        </Box>
        <CandsList
          candsListData={candsPosTypeList}
          candsSource={CandsSourceEnum.PositionType}
          advancedOpen={positionsTypesAdvancedOpen}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <Box mt={1} mr={1} ml={1}>
          <SearchControl
            onSearch={handleFolderCandsSearch}
            onShowAdvanced={() => setFoldersAdvancedOpen(!foldersAdvancedOpen)}
            shoeAdvancedIcon={true}
            records={
              candsStore.folderCandsList && candsStore.folderCandsList.length
            }
            onSort={(sortBy: SortByEnum, dir: string) => {
              handleSort(sortBy, dir, TabsCandsEnum.FolderCands);
            }}
          />
        </Box>
        <CandsList
          candsListData={candsFolderList}
          candsSource={CandsSourceEnum.Folder}
          advancedOpen={foldersAdvancedOpen}
        />
      </div>
    </Paper>
  );
});
