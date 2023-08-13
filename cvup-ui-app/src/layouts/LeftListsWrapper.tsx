import { Box, IconButton, Stack, Tab, Tabs } from "@mui/material";
import { PositionsList } from "../components/positions/PositionsList";
import { useNavigate } from "react-router-dom";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";
import { useStore } from "../Hooks/useStore";
import { observer } from "mobx-react";
import { MdAdd } from "react-icons/md";
import { CrudTypesEnum, TabsGeneralEnum } from "../models/GeneralEnums";
import { SearchControl } from "../components/header/SearchControl";
import { ContactsList } from "../components/contacts/ContactsList";
import { FoldersList } from "../components/folders/FoldersList";
import { ISearchModel } from "../models/GeneralModels";

export const LeftListsWrapper = observer(() => {
  const navigate = useNavigate();
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
    positionsStore.searchPositions(searchVals);
  };

  const handleFoldersSearch = (searchVals: ISearchModel) => {
    foldersStore.searchFolders(searchVals);
  };

  const handleContactsSearch = (searchVals: ISearchModel) => {
    customersContactsStore.searchContacts(searchVals);
  };

  return (
    <Box sx={{ backgroundColor: "white" }}>
      <Box>
        <Stack direction="row" justifyContent="space-between">
          <Tabs
            value={generalStore.currentLeftDrawerTab}
            onChange={handleTabChange}
          >
            <Tab label="Contacts" value={TabsGeneralEnum.Contacts} />
            <Tab label="Folders" value={TabsGeneralEnum.Folders} />
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
            records={positionsStore.positionsSorted.length}
            showSortLeft={true}
            onSortLeftLists={(dir) =>
              (positionsStore.positionsListSortDirection = dir)
            }
          />
        </Box>
        <PositionsList />;
      </Box>
      <Box
        hidden={generalStore.currentLeftDrawerTab !== TabsGeneralEnum.Folders}
      >
        <Box mt={1} ml={1}>
          <SearchControl
            onSearch={handleFoldersSearch}
            records={foldersStore.foldersListSorted.length}
            showSortLeft={true}
            onSortLeftLists={(dir) =>
              (foldersStore.foldersListSortDirection = dir)
            }
          />
        </Box>
        <FoldersList />;
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
        <ContactsList />;
      </Box>
      {/* </ThemeProvider>
        </CacheProvider> */}
    </Box>
  );
});
