import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { LeftListsWrapper } from "./LeftListsWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "../components/cands/CandsListsWrapper";
import { useState } from "react";

type Anchor = "left" | "right";

export const MobileAuthLayout = observer(() => {
  const [drawerState, setDrawerState] = useState({
    left: false,
    right: true,
  });

  const toggleDrawer =
    (anchor: Anchor, open: boolean) =>
    (event: React.KeyboardEvent | React.MouseEvent) => {
      if (
        event.type === "keydown" &&
        ((event as React.KeyboardEvent).key === "Tab" ||
          (event as React.KeyboardEvent).key === "Shift")
      ) {
        return;
      }

      setDrawerState({ ...drawerState, [anchor]: open });
    };

  return (
    <>
      <Drawer
        open={drawerState["left"]}
        anchor="left"
        onClose={toggleDrawer("left", false)}
        sx={{
          zIndex: 9999,
          backgroundColor: "white",
          height: "100vh",
          [`& .MuiDrawer-paper`]: {
            display: "contents",
          },
        }}
      >
        <div style={{ width: "90%" }}>
          <LeftListsWrapper />
        </div>
      </Drawer>
      <Header />
      <Outlet />
      <Drawer
        open={drawerState["right"]}
        anchor="right"
        onClose={toggleDrawer("right", false)}
        sx={{
          zIndex: 9999,
          height: "100vh",
          backgroundColor: "white",
          [`& .MuiDrawer-paper`]: {
            display: "contents",
          },
        }}
      >
        <div style={{ width: "90%", position: "fixed", right: 0 }}>
          <CandsListsWrapper />
        </div>
      </Drawer>
    </>
  );
});
