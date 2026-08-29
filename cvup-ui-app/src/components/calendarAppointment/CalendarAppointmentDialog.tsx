import { Dialog, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { CalendarAppointmentForm } from "./CalendarAppointmentForm";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const CalendarAppointmentDialog = ({ isOpen, onClose }: IProps) => {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog open={open} onClose={onClose} keepMounted PaperProps={{ sx: { minHeight: "60vh", width: "50vw" } }}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        Appointment
      </BootstrapDialogTitle>
      <DialogContent sx={{ pt: 1 }}>
        <CalendarAppointmentForm onClose={onClose} />
      </DialogContent>
    </Dialog>
  );
};
