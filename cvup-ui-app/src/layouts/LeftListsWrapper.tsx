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
import { TabsGeneralEnum } from "../models/GeneralEnums";
import { FoldersListWrapper } from "../components/folders/FoldersListWrapper";
import { ContactsListWrapper } from "../components/contacts/ContactsListWrapper";

export const LeftListsWrapper = observer(() => {
  const navigate = useNavigate();
  const { generalStore } = useStore();

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
        navigate("/position/0");
        break;
      case TabsGeneralEnum.Folders:
        generalStore.showFolderFormDialog = true;
        break;
      case TabsGeneralEnum.Contacts:
        generalStore.showFolderFormDialog = true;
        break;
      default:
        break;
    }
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
            {generalStore.currentTab === TabsGeneralEnum.Positions && (
              <PositionsList />
            )}
            {generalStore.currentTab === TabsGeneralEnum.Folders && (
              <FoldersListWrapper />
            )}
            {generalStore.currentTab === TabsGeneralEnum.Contacts && (
              <ContactsListWrapper />
            )}
          </ThemeProvider>
        </CacheProvider>
      </Box>
    </Box>
  );
});
