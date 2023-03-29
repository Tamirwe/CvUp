import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { PositionForm } from "./PositionForm";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const PositionFormDialog = ({ isOpen, onClose }: IProps) => {
  const { positionsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add Position");

  useEffect(() => {
    if (positionsStore.selectedPosition) {
      setFormTitle("Edit Position");
    }
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"md"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <PositionForm
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
