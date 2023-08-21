import { Dialog, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { ReviewCandForm } from "./ReviewCandForm";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { CustomerReviewCandForm } from "./CustomerReviewCandForm";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const CustomerReviewCandDialog = ({ isOpen, onClose }: IProps) => {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      // onClose={() => onClose(false)}
      fullWidth
      maxWidth={"sm"}
      sx={{}}
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        Customer Review
      </BootstrapDialogTitle>
      <DialogContent>
        <CustomerReviewCandForm
          onSaved={() => {
            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
