import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { LeftListsWrapper } from "./LeftListsWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "../components/cands/CandsListsWrapper";
import { useState } from "react";
import { useStore } from "../Hooks/useStore";

type Anchor = "left" | "right";

export const MobileAuthLayout = observer(() => {
  const { generalStore } = useStore();

  return (
    <>
      <Drawer
        open={generalStore.leftDrawerOpen}
        anchor="left"
        onClose={() => (generalStore.leftDrawerOpen = false)}
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
        open={generalStore.rightDrawerOpen}
        anchor="right"
        onClose={() => (generalStore.rightDrawerOpen = false)}
        sx={{
          zIndex: 9999,
          height: "100vh",
          backgroundColor: "white",
          [`& .MuiDrawer-paper`]: {
            display: "contents",
          },
        }}
      >
        <div
          style={{
            width: "90%",
            position: "fixed",
            right: 0,
            direction: "rtl",
          }}
        >
          <CandsListsWrapper />
        </div>
      </Drawer>
    </>
  );
});
