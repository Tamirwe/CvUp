import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { ReviewCandForm } from "./ReviewCandForm";

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
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"md"}
      sx={{}}
    >
      <DialogTitle>Candidate Review</DialogTitle>
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
