import { Box, Paper, Stack, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../Hooks/useStore";
import { observer } from "mobx-react";
import { SortByEnum, TabsCandsEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { isMobile } from "react-device-detect";
import { PositionCandsList } from "./PositionCandsList";
import { PositionGroupCandsList } from "./PositionGroupCandsList";
import { FolderCandsList } from "./FolderCandsList";
import { AllCandsList } from "./AllCandsList";
import { AiCandsList } from "./AiCandsList";

export const CandsListsTabs = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();
  const [allCandsList, setAllCandsList] = useState<ICand[]>([]);
  const [candsPosList, setCandsPosList] = useState<ICand[]>([]);
  const [candsPosTypeList, setCandsPosTypeList] = useState<ICand[]>([]);
  const [candsFolderList, setCandsFolderList] = useState<ICand[]>([]);

  const sortCandList = (isDesc: boolean, list: ICand[]) => {
    const sorted = list.slice();

    if (isDesc) {
      return sorted.sort((a, b) =>
        a.cvSent > b.cvSent ? 1 : b.cvSent > a.cvSent ? -1 : 0,
      );
    }

    return sorted.sort((a, b) =>
      a.cvSent < b.cvSent ? 1 : b.cvSent < a.cvSent ? -1 : 0,
    );
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

  const handleTabChange = (
    event: React.SyntheticEvent,
    newValue: TabsCandsEnum,
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

  const handleAiSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.AiSearchCands(searchVals.value);
    } else {
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
            positionsStore.selectedPositionType?.id,
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

  const handleSort = (isDesc: boolean, ListSource: TabsCandsEnum) => {
    switch (ListSource) {
      case TabsCandsEnum.AllCands:
        setAllCandsList(sortCandList(isDesc, candsStore.allCandsList));
        break;
      case TabsCandsEnum.AI:
        setAllCandsList(sortCandList(isDesc, candsStore.aiCandsResults));
        break;
      case TabsCandsEnum.PositionTypeCands:
        setAllCandsList(sortCandList(isDesc, candsStore.posTypeCandsList));
        break;
      case TabsCandsEnum.PositionCands:
        setCandsPosList(sortCandList(isDesc, candsStore.posCandsList));
        break;
      case TabsCandsEnum.FolderCands:
        setCandsFolderList(sortCandList(isDesc, candsStore.folderCandsList));
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
                        20,
                      )}
                    </div>
                    <div>Group</div>
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
            <Tab
              label="AI"
              value={TabsCandsEnum.AI}
              sx={{
                alignSelf: "flex-start",
              }}
            />
          </Tabs>
        )}
      </Box>

      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands}>
        <AllCandsList
          allCandsList={allCandsList}
          onSearch={handleAllCandsSearch}
          onSort={(isDesc: boolean) => {
            handleSort(isDesc, TabsCandsEnum.AllCands);
          }}
        />
      </div>

      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AI}>
        <AiCandsList onSearch={handleAiSearch} />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
        <PositionCandsList
          candsPosList={candsPosList}
          onSearch={handlePositionCandsSearch}
          onSort={(isDesc: boolean) => {
            handleSort(isDesc, TabsCandsEnum.PositionCands);
          }}
        />
      </div>
      <div
        hidden={
          candsStore.currentTabCandsLists !== TabsCandsEnum.PositionTypeCands
        }
      >
        <PositionGroupCandsList
          candsPosTypeList={candsPosTypeList}
          onSearch={handlePositionTypeCandsSearch}
          onSort={(isDesc: boolean) => {
            handleSort(isDesc, TabsCandsEnum.PositionTypeCands);
          }}
        />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <FolderCandsList
          candsFolderList={candsFolderList}
          onSearch={handleFolderCandsSearch}
          onSort={(isDesc: boolean) => {
            handleSort(isDesc, TabsCandsEnum.FolderCands);
          }}
        />
      </div>
    </Paper>
  );
});
