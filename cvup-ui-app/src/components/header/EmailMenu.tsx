import { Avatar, Divider, IconButton, Menu, MenuItem } from "@mui/material";
import { useState } from "react";

import { CiMail } from "react-icons/ci";
import { MdOutlineAttachEmail, MdOutlineContactMail } from "react-icons/md";
import { EmailTypeEnum } from "../../models/GeneralEnums";

interface IProps {
  onEmailTypeClick: (emailType: EmailTypeEnum) => void;
}

export const EmailMenu = (props: IProps) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleEmailTypeClick = (emailType: EmailTypeEnum) => {
    props.onEmailTypeClick(emailType);
    setAnchorEl(null);
  };

  return (
    <>
      <IconButton size="medium" onClick={handleClick}>
        <CiMail />
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
            minWidth: "10rem",
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
        <MenuItem onClick={handleClose}>Send Email To:</MenuItem>
        <Divider />
        <MenuItem onClick={() => handleEmailTypeClick(EmailTypeEnum.Contact)}>
          <Avatar sx={{ bgcolor: "#87a8de" }}>
            <MdOutlineAttachEmail />
          </Avatar>
          Contact
        </MenuItem>
        <MenuItem onClick={() => handleEmailTypeClick(EmailTypeEnum.Candidate)}>
          <Avatar sx={{ bgcolor: "#de8787" }}>
            <MdOutlineContactMail />
          </Avatar>
          Candidate
        </MenuItem>
      </Menu>
    </>
  );
};
