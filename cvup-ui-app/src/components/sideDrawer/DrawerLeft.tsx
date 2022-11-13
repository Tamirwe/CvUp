import { Box, Toolbar, Drawer } from "@mui/material";
import { TabsDrawerLeft } from "./TabsDrawerLeft";

interface IProps {
  isOpen: boolean;
}

export const DrawerLeft = ({ isOpen }: IProps) => {
  const drawerWidth = 340;

  return (
    <Drawer
      open={isOpen}
      variant="persistent"
      anchor="left"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: {
          width: drawerWidth,
          boxSizing: "border-box",
        },
      }}
    >
      <Toolbar />
      <Box sx={{ overflow: "auto" }}>
        <TabsDrawerLeft tabSelected={0} />
      </Box>
    </Drawer>
  );
};
