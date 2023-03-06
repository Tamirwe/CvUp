import { Button, Divider, ListItemIcon, Menu, MenuItem } from "@mui/material";
import { useState } from "react";

import { CiLogout } from "react-icons/ci";

interface IProps {
  onMenuSelected: (item: "delete" | "addChild") => void;
}

export const FolderSettingsMenu = (props: IProps) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Button fullWidth variant="text" color="warning" onClick={handleClick}>
        More
      </Button>
      <Menu
        anchorEl={anchorEl}
        id="account-menu"
        open={open}
        onClose={handleClose}
        onClick={handleClose}
        PaperProps={{
          elevation: 0,
          sx: {
            overflow: "visible",
            filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
            mt: 1.5,
            "& .MuiAvatar-root": {
              width: 32,
              height: 32,
              ml: -0.5,
              mr: 1,
            },
            "&:before": {
              content: '""',
              display: "block",
              position: "absolute",
              top: 0,
              right: 14,
              width: 10,
              height: 10,
              bgcolor: "background.paper",
              transform: "translateY(-50%) rotate(45deg)",
              zIndex: 0,
            },
          },
        }}
        transformOrigin={{ horizontal: "right", vertical: "top" }}
        anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
      >
        <MenuItem
          onClick={() => {
            props.onMenuSelected("delete");
            handleClose();
          }}
        >
          <ListItemIcon>
            <CiLogout />
          </ListItemIcon>
          Delete Folder
        </MenuItem>
        <Divider />
        <MenuItem
          onClick={() => {
            props.onMenuSelected("addChild");
            handleClose();
          }}
        >
          <ListItemIcon>
            <CiLogout />
          </ListItemIcon>
          Add Child Folder
        </MenuItem>
      </Menu>
    </>
  );
};
