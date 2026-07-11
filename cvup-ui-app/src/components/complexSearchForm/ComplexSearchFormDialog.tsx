import { Dialog, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { LuceneSearchForm } from "./LuceneSearchForm";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
  positionId?: number;
}

export const ComplexSearchFormDialog = ({ isOpen, onClose, positionId }: IProps) => {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog open={open} onClose={onClose} keepMounted PaperProps={{ sx: { minHeight: "60vh", width: "50vw" } }}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        CV's Search
      </BootstrapDialogTitle>
      <DialogContent sx={{ pt: 1 }}>
        <LuceneSearchForm onClose={onClose} positionId={positionId} />
      </DialogContent>
    </Dialog>
  );
};