import { Box, IconButton, Paper, Stack, Tab, Tabs } from "@mui/material";
import { PositionsList } from "../positions/PositionsList";
import { createTheme } from "@mui/material";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { useStore } from "../../Hooks/useStore";
import { observer } from "mobx-react";
import { MdAdd } from "react-icons/md";
import { CrudTypesEnum, TabsGeneralEnum } from "../../models/GeneralEnums";
import { SearchControl } from "../header/SearchControl";
import { ContactsList } from "../contacts/ContactsList";
import { FoldersList } from "../folders/FoldersList";
import { ISearchModel } from "../../models/GeneralModels";
import { isMobile } from "react-device-detect";
import { PositionsTypesList } from "../positions/PositionsTypesList";

export const LeftListsContainer = observer(() => {
  const { generalStore, foldersStore, customersContactsStore, positionsStore } =
    useStore();

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
    newValue: TabsGeneralEnum
  ) => {
    generalStore.currentLeftDrawerTab = newValue;
  };

  const handleAddClick = () => {
    switch (generalStore.currentLeftDrawerTab) {
      case TabsGeneralEnum.Positions:
        positionsStore.editPosition = undefined;
        generalStore.showPositionFormDialog = true;
        // navigate("/position/0");
        break;
      case TabsGeneralEnum.Folders:
        foldersStore.editFolderSelected = foldersStore.rootFolder;
        generalStore.openModeFolderFormDialog = CrudTypesEnum.Insert;
        break;
      case TabsGeneralEnum.Contacts:
        customersContactsStore.selectedContact = undefined;
        generalStore.showContactFormDialog = true;
        break;
      default:
        break;
    }

    generalStore.leftDrawerOpen = false;
  };

  const handlePositionsSearch = (searchVals: ISearchModel) => {
    positionsStore.searchSortPositions(undefined, searchVals);
  };

  const handleFoldersSearch = (searchVals: ISearchModel) => {
    foldersStore.searchFolders(searchVals);
  };

  const handleContactsSearch = (searchVals: ISearchModel) => {
    customersContactsStore.searchContacts(searchVals);
  };

  return (
    <Paper elevation={3} sx={{ margin: isMobile ? "0" : "0.7rem" }}>
      <Box>
        <Stack direction="row" justifyContent="space-between">
          <Tabs
            scrollButtons
            allowScrollButtonsMobile
            value={generalStore.currentLeftDrawerTab}
            onChange={handleTabChange}
          >
            <Tab label="Contacts" value={TabsGeneralEnum.Contacts} />
            <Tab label="Folders" value={TabsGeneralEnum.Folders} />
            <Tab label="PT" value={TabsGeneralEnum.Types} />
            <Tab label="Positions" value={TabsGeneralEnum.Positions} />
          </Tabs>
          <IconButton
            onClick={handleAddClick}
            size="medium"
            sx={{ height: "fit-content", alignSelf: "center" }}
          >
            <MdAdd />
          </IconButton>
        </Stack>
      </Box>

      {/* <CacheProvider value={cacheRtl}>
          <ThemeProvider theme={themeRtl}> */}
      <Box
        hidden={generalStore.currentLeftDrawerTab !== TabsGeneralEnum.Positions}
      >
        <Box mt={1} ml={1}>
          <SearchControl
            onSearch={handlePositionsSearch}
            records={positionsStore.sortedPosList.length}
            showSortLeft={true}
            onSortLeftLists={(dir) => positionsStore.searchSortPositions(dir)}
          />
        </Box>
        <PositionsList />
      </Box>
      <Box hidden={generalStore.currentLeftDrawerTab !== TabsGeneralEnum.Types}>
        <Box mt={1} ml={1}>
          <SearchControl
            onSearch={handlePositionsSearch}
            records={positionsStore.sortedPosList.length}
            showSortLeft={true}
            onSortLeftLists={(dir) => positionsStore.searchSortPositions(dir)}
          />
        </Box>
        <PositionsTypesList />
      </Box>
      <Box
        hidden={generalStore.currentLeftDrawerTab !== TabsGeneralEnum.Folders}
      >
        <Box mt={1} ml={1}>
          <SearchControl
            onSearch={handleFoldersSearch}
            records={foldersStore.sortedFolders.length}
            showSortLeft={true}
            onSortLeftLists={(dir) => foldersStore.sortFolders(dir)}
          />
        </Box>
        <FoldersList />
      </Box>
      <Box
        hidden={generalStore.currentLeftDrawerTab !== TabsGeneralEnum.Contacts}
      >
        <Box mt={1} ml={1}>
          <SearchControl
            onSearch={handleContactsSearch}
            records={customersContactsStore.contactsListSorted.length}
            showSortLeft={true}
            onSortLeftLists={(dir) =>
              (customersContactsStore.contactsListSortDirection = dir)
            }
          />
        </Box>
        <ContactsList />
      </Box>
      {/* </ThemeProvider>
        </CacheProvider> */}
    </Paper>
  );
});
