import { Box, CssBaseline, Drawer, Toolbar } from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
// import { DrawerLeft } from "../components/drawer/DrawerLeft";
import { styled, useTheme } from "@mui/material/styles";
import useMediaQuery from "@mui/material/useMediaQuery";

const drawerWidth = 340;

const Main = styled("main", {
  shouldForwardProp: (prop) => prop !== "isOpen" && prop !== "matches",
})<{
  isOpen?: boolean;
  matches?: boolean;
}>(({ theme, isOpen, matches }) => ({
  flexGrow: 1,
  padding: theme.spacing(3),
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

export const LayoutAuth = () => {
  const matches = useMediaQuery("(min-width:600px)");
  const [isOpen, setIsOpen] = useState(matches ? true : false);

  const handleDrawerToggle = () => {
    setIsOpen(!isOpen);
  };

  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />
      <Header onToggleDrawer={handleDrawerToggle} />
      {matches ? (
        <DrawerDesktop isOpen={isOpen} />
      ) : (
        <DrawerMobile isOpen={isOpen} onClose={() => setIsOpen(false)} />
      )}

      <Main isOpen={isOpen} matches={matches}>
        <Toolbar />
        <Outlet />
      </Main>
    </Box>
  );
};

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
        },
      }}
    >
      <Toolbar />
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
    </Drawer>
  );
};
