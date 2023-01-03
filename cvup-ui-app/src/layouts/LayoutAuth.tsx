import { Box, Drawer, Grid, Toolbar } from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import useMediaQuery from "@mui/material/useMediaQuery";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";
import { observer } from "mobx-react";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { QuillRte } from "../components/rte/QuillRte";
import { useStore } from "../Hooks/useStore";

const drawerWidth = 340;

export const LayoutAuth = observer(() => {
  const { generalStore } = useStore();
  const matches = useMediaQuery("(min-width:600px)");
  const [isOpen, setIsOpen] = useState(matches ? true : false);

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Grid container spacing={0} columns={18}>
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
            <PositionsListWrapper />
          </Drawer>
        </Grid>
        <Grid item xs={14}>
          <Header />
          <Grid container spacing={0}>
            <Grid item xs={7}>
              <Outlet />
            </Grid>
            <Grid item xs={5}>
              <Drawer
                open={isOpen}
                variant="persistent"
                anchor="right"
                sx={{
                  [`& .MuiDrawer-paper`]: {
                    display: "contents",
                  },
                }}
              >
                <CvsListWrapper />
                <QuillRte />
              </Drawer>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Box>
  );
});
