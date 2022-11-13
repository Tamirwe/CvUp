import { Box, CssBaseline } from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { TopBar } from "../components/header/TopBar";
import { DrawerLeft } from "../components/sideDrawer/DrawerLeft";

export const LayoutAuth = () => {
  const [isOpen, setIsOpen] = useState(true);

  const handleDrawerToggle = () => {
    setIsOpen(!isOpen);
  };

  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />
      <TopBar onToggleDrawer={handleDrawerToggle} />
      <DrawerLeft isOpen={isOpen} />
      <Outlet />
    </Box>

    // <div>
    //   <Button
    //     fullWidth
    //     size="large"
    //     type="submit"
    //     variant="contained"
    //     color="secondary"

    //   >
    //     Logout
    //   </Button>
    //   <div>Layout Auth</div>
    //   <Outlet />
    // </div>
  );
};
