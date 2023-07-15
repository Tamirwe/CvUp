import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { LeftListsWrapper } from "./LeftListsWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "../components/cands/CandsListsWrapper";

export const BrowserAuthLayout = observer(() => {
  return (
    <Box sx={{ flexGrow: 1 }}>
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
            <LeftListsWrapper />
          </Drawer>
        </Grid>
        <Grid
          item
          xs={8}
          columns={12}
          sx={{ backgroundColor: "#fff", paddingLeft: "10px" }}
        >
          <Header />
          <Grid container spacing={0} sx={{ borderRadius: "6px" }}>
            <Grid item xs={12}>
              <Outlet />
            </Grid>
          </Grid>
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
    </Box>
  );
});
