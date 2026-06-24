import { Box, Paper, Stack, Tab, Tabs } from "@mui/material";
import { observer } from "mobx-react";
import { useStore } from "../../Hooks/useStore";
import { TabsCandsEnum } from "../../models/GeneralEnums";
import { isMobile } from "react-device-detect";
import { PositionCandsList } from "../PositionCandsList";
import { PositionGroupCandsList } from "../PositionGroupCandsList";
import { FolderCandsList } from "../FolderCandsList";
import { AllCandsList } from "../AllCandsList";
import { AiCandsList } from "../AiCandsList";

export const CandsListsTabs = observer(() => {
  const { candsStore, positionsStore, foldersStore } = useStore();

  const handleTabChange = (
    event: React.SyntheticEvent,
    newValue: TabsCandsEnum,
  ) => {
    event.stopPropagation();
    event.preventDefault();
    candsStore.currentTabCandsLists = newValue;
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
                display: "none",
              }}
            />
          </Tabs>
        )}
      </Box>

      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AllCands}>
        <AllCandsList />
      </div>

      <div hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.AI}>
        <AiCandsList />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.PositionCands}
      >
        <PositionCandsList />
      </div>
      <div
        hidden={
          candsStore.currentTabCandsLists !== TabsCandsEnum.PositionTypeCands
        }
      >
        <PositionGroupCandsList />
      </div>
      <div
        hidden={candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands}
      >
        <FolderCandsList />
      </div>
    </Paper>
  );
});
