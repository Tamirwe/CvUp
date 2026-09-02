import { useStore } from "../../Hooks/useStore";
import { Grid, IconButton, Link, Stack } from "@mui/material";
import { SettingsMenu } from "./SettingsMenu";
import { useCallback, useEffect } from "react";
import { BsList } from "react-icons/bs";
import { isMobile } from "react-device-detect";
import { observer } from "mobx-react";
import { useLocation } from "react-router-dom";

export const Header = observer(() => {
  const location = useLocation();

  const {
    candsStore,
    generalStore,
    positionsStore,
    customersContactsStore,
    authStore,
  } = useStore();

  useEffect(() => {
    (async () => {
      await Promise.all([
        candsStore.getCandsList(),
        candsStore.getCandPosStages(),
        positionsStore.getPositionsList(),
        positionsStore.getPositionsTypesList(),
        candsStore.getEmailTemplates(),
        customersContactsStore.getCustomersList(),
        authStore.getUser(),
        candsStore.getSearches(),
      ]);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handlePositionClick = useCallback(
    async (event: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => {
      await positionsStore.positionClick(
        positionsStore.candDisplayPosition!.id,
        true,
      );
      candsStore.setDisplayCandOntopPCList();
    },
    [],
  );

  return (
    <div
      data-role="header"
      data-position="fixed"
      style={{
        position: "sticky",
        height: "2.8rem",
        width: "100%",
        zIndex: 1290,
        backgroundColor: "#fff",
        top: 0,
        left: 0,
      }}
    >
      <div
        style={{
          margin: "5px 10px",
          height: "100%",
          display: "flex",
          alignItems: "center",
          borderRadius: "25px",
        }}
      >
        <Grid container spacing={0}>
          <Grid item xs={4} sm={7} pl={1}>
            <Stack direction="row" alignItems="center" spacing={1}>
              {isMobile && (
                <IconButton
                  size="medium"
                  onClick={() => {
                    generalStore.rightDrawerOpen = false;
                    generalStore.leftDrawerOpen = !generalStore.leftDrawerOpen;
                  }}
                >
                  <BsList />
                </IconButton>
              )}

              <SettingsMenu />
            </Stack>
          </Grid>
          <Grid
            item
            xs={8}
            sm={5}
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "flex-end",
            }}
            pr={1}
          >
            {candsStore.candDisplay &&
              location.pathname === "/cv" &&
              positionsStore.candDisplayPosition && (
                <Link
                  href="#"
                  onClick={handlePositionClick}
                  dir="rtl"
                  sx={{
                    flexGrow: 1,
                    minWidth: 0,
                    color: "#7b84ff",
                    fontWeight: 700,
                    fontSize: isMobile ? "0.9rem" : "1.1rem",
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    textAlign: "right",
                  }}
                >
                  {positionsStore.candDisplayPosition?.name || ""}
                  &nbsp;-&nbsp;
                  {positionsStore.candDisplayPosition?.customerName || ""}
                </Link>
              )}
            {isMobile && (
              <IconButton
                size="medium"
                onClick={() => {
                  generalStore.leftDrawerOpen = false;
                  generalStore.rightDrawerOpen = !generalStore.rightDrawerOpen;
                }}
              >
                <BsList />
              </IconButton>
            )}
          </Grid>
        </Grid>
      </div>
    </div>
  );
});
