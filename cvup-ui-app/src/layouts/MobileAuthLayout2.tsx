import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsListsContainer } from "../components/containers/CandsListsContainer";
import { LeftListsContainer } from "../components/containers/LeftListsContainer";

type Anchor = "left" | "right";

export const MobileAuthLayout2 = observer(() => {
  const { generalStore } = useStore();

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
