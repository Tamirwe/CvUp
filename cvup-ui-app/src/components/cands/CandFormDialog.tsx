import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { CandForm } from "./CandForm";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const CandFormDialog = ({ isOpen, onClose }: IProps) => {
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
      sx={{
        "& .MuiDialog-container": {
          alignItems: "flex-start",
          justifyContent: "left",
          padding: 2,
        },
      }}
    >
      <DialogTitle>Candidate Details</DialogTitle>
      <DialogContent>
        <CandForm
          onSaved={() => {
            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
