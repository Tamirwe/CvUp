import {
  Box,
  Toolbar,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { useState } from "react";
import { TabsDrawerLeft } from "./TabsDrawerLeft";
import { MdMenu, MdLogout } from "react-icons/md";

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
      <Box sx={{ overflow: "auto", display: "flex" }}>
        <List>
          {["Inbox", "Starred", "Send email", "Drafts"].map((text, index) => (
            <ListItem key={text} disablePadding sx={{ display: "block" }}>
              <ListItemButton
                sx={{
                  minHeight: 48,
                  px: 2.5,
                }}
              >
                <ListItemIcon
                  sx={{
                    minWidth: 0,
                    justifyContent: "center",
                  }}
                >
                  {index % 2 === 0 ? <MdLogout /> : <MdMenu />}
                </ListItemIcon>
              </ListItemButton>
            </ListItem>
          ))}
        </List>
        <TabsDrawerLeft tabSelected={0} />
      </Box>
    </Drawer>
  );
};
