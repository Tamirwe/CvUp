import { Drawer } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { LeftListsWrapper } from "./LeftListsWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "../components/cands/CandsListsWrapper";
import { useStore } from "../Hooks/useStore";

type Anchor = "left" | "right";

export const MobileAuthLayout = observer(() => {
  const { generalStore } = useStore();

  return (
    <>
      <Drawer
        variant="persistent"
        open={true}
        anchor="left"
        onClose={() => (generalStore.leftDrawerOpen = false)}
        sx={{
          zIndex: 9999,
          backgroundColor: "white",
          // height: "100vh",
          [`& .MuiDrawer-paper`]: {
            display: "contents",
          },
        }}
      >
        <div
          style={{
            width: "100%",
            position: "fixed",
            left: generalStore.leftDrawerOpen ? 0 : "-500px",
            height: "100vh",
            top: 0,
            zIndex: "9999",
            backgroundColor: "#0000002e",
          }}
          onClick={(event) => {
            event.stopPropagation();
            event.preventDefault();
            generalStore.leftDrawerOpen = false;
          }}
        >
          <div
            onClick={(event) => {
              event.stopPropagation();
              event.preventDefault();
            }}
            style={{
              position: "fixed",
              left: generalStore.leftDrawerOpen ? 0 : "-500px",
              zIndex: "99999",
              width: "90%",
            }}
          >
            <LeftListsWrapper />
          </div>
        </div>
      </Drawer>
      <Header />
      <Outlet />
      <Drawer
        variant="persistent"
        open={true}
        anchor="right"
        onClose={() => (generalStore.rightDrawerOpen = false)}
        sx={{
          // height: "100vh",
          [`& .MuiDrawer-paper`]: {
            display: "contents",
          },
        }}
      >
        <div
          style={{
            width: "100%",
            position: "fixed",
            right: generalStore.rightDrawerOpen ? 0 : "-500px",
            top: 0,
            direction: "rtl",
            zIndex: "9999",
            backgroundColor: "#0000002e",
          }}
          onClick={(event) => {
            event.stopPropagation();
            event.preventDefault();
            generalStore.rightDrawerOpen = false;
          }}
        >
          <div
            // onClick={(event) => {
            //   event.stopPropagation();
            //   event.preventDefault();
            // }}
            style={{
              overflow: "hidden",
              zIndex: "99999",
              width: "90%",
            }}
          >
            <CandsListsWrapper />
          </div>
        </div>
      </Drawer>
    </>
  );
});
