import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { AppBar, Toolbar, IconButton, Typography } from "@mui/material";
import { MdMenu, MdLogout } from "react-icons/md";
import { useNavigate } from "react-router-dom";

interface IProps {
  onToggleDrawer: () => void;
}

export const DrawerToolbar = (props: IProps) => {
  const { authStore } = useStore();

  useEffect(() => {}, []);
  const navigate = useNavigate();

  return (
    <AppBar
      position="relative"
      sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}
    >
      <Toolbar>
        <IconButton
          size="large"
          edge="start"
          color="inherit"
          aria-label="menu"
          sx={{ mr: 2 }}
          onClick={(e) => {
            props.onToggleDrawer();
          }}
        >
          <MdMenu />
        </IconButton>
        <Typography
          variant="h6"
          component="div"
          sx={{ flexGrow: 1 }}
        ></Typography>
        <IconButton
          color="inherit"
          aria-label="add an alarm"
          onClick={(e) => {
            authStore.logout();
            navigate("/login");
          }}
        >
          <MdLogout />
        </IconButton>
      </Toolbar>
    </AppBar>
  );
};
