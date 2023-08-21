import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { observer } from "mobx-react";
import { CandsListsContainer } from "../components/containers/CandsListsContainer";
import { LeftListsContainer } from "../components/containers/LeftListsContainer";
import logo from "../assets/images/CvUpLogo2.png";

export const DescktopAuthLayout = observer(() => {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <Grid container spacing={0}>
        <Grid item xs={8}>
          <Grid container columns={24} spacing={0}>
            <Grid item xs={9}>
              <Grid container spacing={0}>
                <Grid item xs={12}>
                  <div
                    style={{
                      height: "2.78rem",
                      padding: "1.0rem 0rem 0rem 1rem",
                      fontSize: "1.8rem",
                      color: "#087d56",
                      position: "relative",
                    }}
                  >
                    <img
                      src={logo}
                      style={{
                        width: "5rem",
                        position: "absolute",
                        top: "5px",
                      }}
                    />
                  </div>
                </Grid>
              </Grid>
              <LeftListsContainer />
            </Grid>
            <Grid item xs={15} sx={{ backgroundColor: "#fff" }}>
              <Grid container spacing={0}>
                <Grid item xs={12}>
                  <Header />
                </Grid>
              </Grid>
              <Outlet />
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={4} sx={{ direction: "rtl" }}>
          <CandsListsContainer />
        </Grid>
      </Grid>
    </Box>
  );
});
