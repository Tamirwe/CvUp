import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { CustomerForm } from "./CustomerForm";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const CustomerFormDialog = ({ isOpen, onClose }: IProps) => {
  const { customersContactsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add Customer");

  useEffect(() => {
    if (customersContactsStore.selectedCustomer) {
      setFormTitle("Edit Customer");
    }
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"xs"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <CustomerForm
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
