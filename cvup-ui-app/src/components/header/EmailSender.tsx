import { Dialog, DialogContent, DialogTitle, IconButton } from "@mui/material";
import { useState } from "react";
import { CiMail } from "react-icons/ci";

export const EmailSender = () => {
  const [isOpen, setIsOpen] = useState(false);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setIsOpen(true);
  };

  const handleClose = () => {
    setIsOpen(false);
  };

  return (
    <>
      <IconButton size="medium" onClick={handleClick}>
        <CiMail />
      </IconButton>
      {isOpen && (
        <Dialog open={isOpen} onClose={handleClose} fullWidth maxWidth={"xs"}>
          <DialogTitle>Send Email</DialogTitle>
          <DialogContent></DialogContent>
        </Dialog>
      )}
    </>
  );
};
