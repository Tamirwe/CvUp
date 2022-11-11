import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../../Hooks/useStore";
import {
  Box,
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  Button,
} from "@mui/material";
import { MdMenu } from "react-icons/md";

export const TopBar = observer(() => {
  const { authStore } = useStore();

  const [id, setId] = useState(0);

  useEffect(() => {}, []);

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static">
        <Toolbar>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{ mr: 2 }}
          >
            <MdMenu />
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            News
          </Typography>
          <Button color="inherit">Login</Button>
        </Toolbar>
      </AppBar>
    </Box>
  );
});
