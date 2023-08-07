import {
  Avatar,
  Divider,
  IconButton,
  ListItemAvatar,
  ListItemIcon,
  Menu,
  MenuItem,
} from "@mui/material";
import { useState } from "react";

import { CiLogout, CiSettings } from "react-icons/ci";
import {
  MdDelete,
  MdGroup,
  MdOutlineDelete,
  MdOutlineTextSnippet,
} from "react-icons/md";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  UserRoleEnum,
} from "../../models/GeneralEnums";
import { green } from "@mui/material/colors";

export const SettingsMenu = () => {
  const { generalStore, authStore, candsStore } = useStore();
  const navigate = useNavigate();

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleReports = () => {
    navigate(`/`);
    setAnchorEl(null);
  };

  return (
    <>
      <IconButton size="medium" onClick={handleClick}>
        <CiSettings />
      </IconButton>
      <Menu
        anchorEl={anchorEl}
        id="account-menu"
        open={open}
        onClose={handleClose}
        onClick={handleClose}
        PaperProps={{
          elevation: 0,
          sx: {
            minWidth: "13rem",
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
        <MenuItem onClick={handleClose}>
          <Avatar />{" "}
          {`${authStore.currentUser?.firstName} ${authStore.currentUser?.lastName}`}
        </MenuItem>
        <MenuItem onClick={handleReports}>
          <Avatar sx={{ bgcolor: green[400] }}>
            <MdOutlineTextSnippet style={{ color: "#fff" }} />
          </Avatar>{" "}
          Reports
        </MenuItem>
        {/* <MenuItem onClick={handleClose}>
          <Avatar /> My account
        </MenuItem> */}
        <Divider />
        <MenuItem
          onClick={async () => {
            const isDelete = await generalStore.alertConfirmDialog(
              AlertConfirmDialogEnum.Confirm,
              "Delete Cv",
              "Are you sure you want to delete this cv?"
            );

            if (isDelete) {
              await candsStore.deleteCv();
            }
          }}
        >
          <Avatar>
            <MdOutlineDelete />
          </Avatar>
          Delete Cv
        </MenuItem>
        <MenuItem
          onClick={async () => {
            const isDelete = await generalStore.alertConfirmDialog(
              AlertConfirmDialogEnum.Confirm,
              "Delete Candidate",
              "Are you sure you want to delete this candidate?"
            );

            if (isDelete) {
              await candsStore.deleteCandidate();
            }
          }}
        >
          <Avatar>
            <MdDelete />
          </Avatar>
          Delete Candidate
        </MenuItem>

        <Divider />

        {/* {authStore.userRole === UserRoleEnum.Admin && ( */}
        <MenuItem
          onClick={() => {
            generalStore.showUserListDialog = true;
          }}
        >
          <Avatar>
            <MdGroup />
          </Avatar>
          Users
        </MenuItem>
        <MenuItem
          onClick={() => {
            generalStore.showCustomersListDialog = true;
          }}
        >
          <Avatar>
            <MdGroup />
          </Avatar>
          Customers
        </MenuItem>
        {/* )} */}
        <MenuItem
          onClick={() => {
            authStore.logout();
            navigate("/login");
          }}
        >
          <ListItemIcon>
            <CiLogout />
          </ListItemIcon>
          Logout
        </MenuItem>
      </Menu>
    </>
  );
};
