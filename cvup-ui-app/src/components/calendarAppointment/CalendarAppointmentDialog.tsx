import { Dialog, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { CalendarAppointmentForm } from "./CalendarAppointmentForm";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const CalendarAppointmentDialog = ({ isOpen, onClose }: IProps) => {
  const { customersContactsStore } = useStore();
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);

    // The contacts are loaded by the contacts page only,
    // so without them the contacts control has nothing to offer
    if (isOpen && !customersContactsStore.contactsList.length) {
      customersContactsStore.getContactsList();
    }
  }, [isOpen]); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <Dialog open={open} onClose={onClose} keepMounted PaperProps={{ sx: { minHeight: "60vh", width: "50vw" } }}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        Appointment
      </BootstrapDialogTitle>
      <DialogContent sx={{ pt: 1 }}>
        <CalendarAppointmentForm isOpen={isOpen} onClose={onClose} />
      </DialogContent>
    </Dialog>
  );
};
