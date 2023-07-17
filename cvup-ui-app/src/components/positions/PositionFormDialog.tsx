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
    if (positionsStore.editPosition) {
      setFormTitle("Edit Position");
    }
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"lg"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <PositionForm
          onSaved={async (posId) => {
            await positionsStore.getPositionsList();

            if (posId > 0) {
              positionsStore.setPosSelected(posId);
            } else {
              positionsStore.selectedPosition = undefined;
            }

            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
