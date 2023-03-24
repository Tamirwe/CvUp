import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { UserForm } from "./UserForm";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const UsersFormDialog = ({ isOpen, onClose }: IProps) => {
  const { authStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add User");

  useEffect(() => {
    if (authStore.selectedUser) {
      setFormTitle("Edit User");
    }

    setOpen(isOpen);
  }, [isOpen]);

  const handleSave = () => {
    authStore.getUsersList();
    onClose();
  };

  return (
    <Dialog open={open} onClose={() => onClose()} fullWidth maxWidth={"sm"}>
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <UserForm onSaved={handleSave} onCancel={() => onClose()} />
      </DialogContent>
    </Dialog>
  );
};
