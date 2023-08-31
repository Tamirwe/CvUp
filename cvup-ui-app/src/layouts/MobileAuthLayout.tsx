import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsListsContainer } from "../components/containers/CandsListsContainer";
import { LeftListsContainer } from "../components/containers/LeftListsContainer";
import { useEffect } from "react";
import { isMobile } from "react-device-detect";

type Anchor = "left" | "right";

export const MobileAuthLayout = observer(() => {
  const { generalStore } = useStore();

  useEffect(() => {
    if (isMobile) {
      if (generalStore.rightDrawerOpen || generalStore.leftDrawerOpen) {
        document.body.style.overflow = "hidden";
      } else {
        document.body.style.removeProperty("overflow");
      }
    }
  }, [generalStore.rightDrawerOpen, generalStore.leftDrawerOpen]);

  return (
    <>
      <Header />
      <Outlet />
      <div
        style={{
          display: generalStore.leftDrawerOpen ? "block" : "none",
          width: "100%",
          position: "fixed",
          top: "3rem",
        }}
      >
        <LeftListsContainer />
      </div>
      <div
        style={{
          display: generalStore.rightDrawerOpen ? "block" : "none",
          width: "100%",
          position: "fixed",
          top: "3rem",
        }}
      >
        <CandsListsContainer />
      </div>
    </>
  );
});
