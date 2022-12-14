import { Box, Drawer, Grid, Toolbar } from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import useMediaQuery from "@mui/material/useMediaQuery";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";
import { observer } from "mobx-react";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { CvReview } from "../components/cvs/CvReview";
import { useStore } from "../Hooks/useStore";

const drawerWidth = 340;

export const LayoutAuth = observer(() => {
  const { cvsStore } = useStore();
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
        <Grid
          item
          xs={14}
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
                {cvsStore.openCvReviewDialogOpen && <CvReview />}
              </Drawer>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Box>
  );
});
