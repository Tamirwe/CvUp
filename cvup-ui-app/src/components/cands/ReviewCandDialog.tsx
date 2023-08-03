import { Dialog, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { ReviewCandForm } from "./ReviewCandForm";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const ReviewCandDialog = ({ isOpen, onClose }: IProps) => {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      // onClose={() => onClose(false)}
      fullWidth
      maxWidth={"md"}
      sx={{}}
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        Candidate Review
      </BootstrapDialogTitle>
      <DialogContent>
        <ReviewCandForm
          onSaved={() => {
            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
