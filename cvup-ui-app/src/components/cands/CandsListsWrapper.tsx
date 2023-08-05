import { Box, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CandsList } from "./CandsList";
import { observer } from "mobx-react";
import {
  CandsSourceEnum,
  SortByEnum,
  TabsCandsEnum,
} from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { ICand, ISearchModel } from "../../models/GeneralModels";

export const CandsListsWrapper = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
  // const [folderId, setFolderId] = useState(0);
  const [allCandsList, setAllCandsList] = useState<ICand[]>([]);
  const [candsPosList, setCandsPosList] = useState<ICand[]>([]);
  const [candsFolderList, setCandsFolderList] = useState<ICand[]>([]);

  const [allCandsListSort, setAllCandsListSort] = useState("");
  const [candsPosListSort, setCandsPosListSort] = useState("");
  const [candsFolderListSort, setCandsFolderListSort] = useState("");

  const [candsAdvancedOpen, setCandsAdvancedOpen] = useState(false);
  const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);
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
        if (positionsStore.selectedPosition?.id) {
          candsStore.getPositionCandsList(positionsStore.selectedPosition?.id);
          candsStore.setDisplayCandOntopPCList();
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

      default:
        break;
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
        <Box mt={1} mr={1} ml={1}>
          <SearchControl
            onSearch={handleAllCandsSearch}
            onShowAdvanced={() => setCandsAdvancedOpen(!candsAdvancedOpen)}
            shoeAdvancedIcon={true}
            records={candsStore.allCandsList.length}
            onSort={(sortBy: SortByEnum, dir: string) => {
              handleSort(sortBy, dir, TabsCandsEnum.AllCands);
            }}
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
        <Box mt={1} mr={2}>
          <SearchControl
            onSearch={handlePositionCandsSearch}
            onShowAdvanced={() =>
              setPositionsAdvancedOpen(!positionsAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={candsStore.posCandsList.length}
            onSort={(sortBy: SortByEnum, dir: string) => {
              setCandsPosListSort(dir);
            }}
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
            records={candsStore.folderCandsList.length}
            onSort={(sortBy: SortByEnum, dir: string) => {
              setCandsFolderListSort(dir);
            }}
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
