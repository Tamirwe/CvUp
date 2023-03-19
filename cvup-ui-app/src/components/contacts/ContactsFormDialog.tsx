import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { IContact } from "../../models/GeneralModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { ContactsForm } from "./ContactsForm";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  crudType?: CrudTypesEnum;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const ContactsFormDialog = ({
  isOpen,
  onClose,
}: IProps) => {
  const { customersContactsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add Contact");

  useEffect(() => {
    if (customersContactsStore.selectedContact) {
      setFormTitle("Edit Contact");
    }
    setOpen(isOpen);
  }, [isOpen]);

  const handleSave=()=>{
    customersContactsStore.getContactsList();
    onClose(false);
  }

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"xs"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <ContactsForm
          onSaved={handleSave}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
