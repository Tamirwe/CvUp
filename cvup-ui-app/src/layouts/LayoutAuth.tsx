import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "../components/cands/CandsListsWrapper";
import { CandReview } from "../components/cands/CandReview";
import { useStore } from "../Hooks/useStore";

export const LayoutAuth = observer(() => {
  const { candsStore } = useStore();

  return (
    <Box sx={{ flexGrow: 1 }}>
      {candsStore.cvReviewDialogOpen && <CandReview />}
      <Grid container spacing={0} columns={18}>
        <Grid item xs={5}>
          <Drawer
            open={true}
            variant="persistent"
            anchor="left"
            sx={{
              [`& .MuiDrawer-paper`]: {
                display: "contents",
              },
            }}
          >
            <PositionsListWrapper />
          </Drawer>
        </Grid>
        <Grid
          item
          xs={13}
          columns={12}
          sx={{ backgroundColor: "#fff", paddingLeft: "10px" }}
        >
          <Header />
          <Grid
            container
            spacing={0}
            sx={{ marginTop: "58px", borderRadius: "6px" }}
          >
            <Grid item xs={7}>
              <Outlet />
            </Grid>
            <Grid item xs={5}>
              <Drawer
                open={true}
                variant="persistent"
                anchor="right"
                sx={{
                  [`& .MuiDrawer-paper`]: {
                    display: "contents",
                  },
                }}
              >
                <CandsListsWrapper />
              </Drawer>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Box>
  );
});
