import {
  Backdrop,
  Box,
  CircularProgress,
  CssBaseline,
  Drawer,
  Grid,
  Tab,
  Tabs,
  Toolbar,
} from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
// import { DrawerLeft } from "../components/drawer/DrawerLeft";
import { styled } from "@mui/material/styles";
import useMediaQuery from "@mui/material/useMediaQuery";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";

const drawerWidth = 340;

const Main = styled("main", {
  shouldForwardProp: (prop) => prop !== "isOpen" && prop !== "matches",
})<{
  isOpen?: boolean;
  matches?: boolean;
}>(({ theme, isOpen, matches }) => ({
  flexGrow: 1,
  padding: theme.spacing(1),
  transition: theme.transitions.create("margin", {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  marginLeft: `-${matches ? drawerWidth : 0}px`,
  ...(isOpen && {
    transition: theme.transitions.create("margin", {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginLeft: 0,
  }),
}));

export const LayoutAuth = observer(() => {
  const { generalStore } = useStore();
  const matches = useMediaQuery("(min-width:600px)");
  const [isOpen, setIsOpen] = useState(matches ? true : false);

  const handleDrawerToggle = () => {
    setIsOpen(!isOpen);
  };

  const [value, setValue] = useState(0);
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Grid container spacing={0} columns={16}>
        <Grid item xs={4}>
          <Drawer
            open={isOpen}
            variant="persistent"
            anchor="left"
            sx={{
              [`& .MuiDrawer-paper`]: {
                display: "contents",
              },
            }}
          >
            <Grid container spacing={0}>
              <Grid item xs={10}>
                <PositionsListWrapper />
              </Grid>
              <Grid item xs={2}>
                <Tabs
                  orientation="vertical"
                  variant="scrollable"
                  value={value}
                  onChange={handleChange}
                  aria-label="Vertical tabs example"
                  sx={{ borderRight: 1, borderColor: "divider" }}
                >
                  <Tab label="Item One" />
                  <Tab label="Item Two" />
                  <Tab label="Item Three" />
                  <Tab label="Item Four" />
                </Tabs>
              </Grid>
            </Grid>
          </Drawer>
        </Grid>
        <Grid item xs={8}>
          <Outlet />
        </Grid>
        <Grid item xs={4}>
          <CvsListWrapper />
        </Grid>
      </Grid>
    </Box>

    // <Box sx={{ display: "flex" }}>
    //   <CssBaseline />
    //   {/* <Header onToggleDrawer={handleDrawerToggle} /> */}
    //   {/* {matches ? (
    //     <DrawerDesktop isOpen={isOpen} />
    //   ) : (
    //     <DrawerMobile isOpen={isOpen} onClose={() => setIsOpen(false)} />
    //   )} */}

    //   <Main isOpen={isOpen} matches={matches} sx={{ p: 0 }}>
    //     {/* <Toolbar /> */}
    //     <Outlet />
    //   </Main>
    //   <Backdrop
    //     sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
    //     open={generalStore.backdrop}
    //     onClick={() => {
    //       generalStore.backdrop = false;
    //     }}
    //   >
    //     <CircularProgress color="inherit" />
    //   </Backdrop>
    // </Box>
  );
});

interface IPropsDrawerDesktop {
  isOpen: boolean;
}

const DrawerDesktop = ({ isOpen }: IPropsDrawerDesktop) => {
  const drawerWidth = 340;

  return (
    <Drawer
      open={isOpen}
      variant="persistent"
      anchor="left"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: {
          width: drawerWidth,
          boxSizing: "border-box",
          overflow: "hidden",
        },
      }}
    >
      {/* <Toolbar /> */}
      <PositionsListWrapper />
    </Drawer>
  );
};

interface IPropsDrawerMobile {
  isOpen: boolean;
  onClose: () => void;
}

const DrawerMobile = ({ isOpen, onClose }: IPropsDrawerMobile) => {
  const drawerWidth = 340;

  return (
    <Drawer
      open={isOpen}
      anchor="left"
      onClose={() => onClose()}
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: {
          width: drawerWidth,
          boxSizing: "border-box",
        },
      }}
    >
      <Toolbar />
      <PositionsListWrapper />
    </Drawer>
  );
};
