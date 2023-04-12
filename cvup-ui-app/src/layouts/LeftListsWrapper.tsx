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
    generalStore.currentTab = newValue;
  };

  const handleAddClick = () => {
    switch (generalStore.currentTab) {
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
  };

  const handlePositionsSearch = (val: string) => {
    positionsStore.searchPositions();
  };

  const handleFoldersSearch = (val: string) => {
    foldersStore.searchFolders();
  };

  const handleContactsSearch = (val: string) => {
    customersContactsStore.searchContacts();
  };

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Stack direction="row" justifyContent="space-between">
          <Tabs value={generalStore.currentTab} onChange={handleTabChange}>
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
      <Box
        sx={{
          height: "83vh",
          // display: "flex",
          // flexDirection: "column",
          // flexWrap: "wrap",
          // "& > :not(style)": {
          //   m: 1,
          // },
        }}
      >
        <CacheProvider value={cacheRtl}>
          <ThemeProvider theme={themeRtl}>
            <div hidden={generalStore.currentTab !== TabsGeneralEnum.Positions}>
              <Box>
                <Box mt={1} ml={1}>
                  <SearchControl onSearch={handlePositionsSearch} />
                </Box>
                <PositionsList />;
              </Box>
            </div>
            <div hidden={generalStore.currentTab !== TabsGeneralEnum.Folders}>
              <Box>
                <Box mt={1} ml={1}>
                  <SearchControl onSearch={handleFoldersSearch} />
                </Box>
                <FoldersList />;
              </Box>
            </div>
            <div hidden={generalStore.currentTab !== TabsGeneralEnum.Contacts}>
              <Box>
                <Box mt={1} ml={1}>
                  <SearchControl onSearch={handleContactsSearch} />
                </Box>
                <ContactsList />;
              </Box>
            </div>
          </ThemeProvider>
        </CacheProvider>
      </Box>
    </Box>
  );
});
