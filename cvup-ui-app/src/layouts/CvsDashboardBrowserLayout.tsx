import { Box, Drawer, Grid } from "@mui/material";
import { Outlet, useNavigate } from "react-router-dom";
import { Header } from "../components/header/Header";
import { observer } from "mobx-react";
import { CandsListsTabs } from "../candsLists/tabs/CandsListsTabs";
import { LeftListsContainer } from "../components/containers/LeftListsContainer";
import logo from "../assets/images/CvUpLogo2.png";

export const CvsDashboardBrowserLayout = observer(() => {
  const navigate = useNavigate();

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Grid container columns={24} spacing={0}>
        <Grid item xs={17}>
          <Grid container columns={24} spacing={0}>
            <Grid item xs={8}>
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
                    {/* The logo is the switch between the two auth layouts:
                        from here it goes to the admin dashboard, and the logo
                        there comes back. */}
                    <img
                      src={logo}
                      alt="CvUp"
                      title="Admin dashboard"
                      onClick={() => navigate("/admin")}
                      style={{
                        width: "5rem",
                        position: "absolute",
                        top: "5px",
                        cursor: "pointer",
                      }}
                    />
                  </div>
                </Grid>
              </Grid>
              <LeftListsContainer />
            </Grid>
            <Grid item xs={16} sx={{ backgroundColor: "#fff" }}>
              <Grid container spacing={0}>
                <Grid item xs={12}>
                  <Header />
                </Grid>
              </Grid>
              <Outlet />
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={7} sx={{ direction: "rtl" }}>
          <CandsListsTabs />
        </Grid>
      </Grid>
    </Box>
  );
});
