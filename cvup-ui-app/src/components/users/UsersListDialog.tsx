import {
  Dialog,
  DialogTitle,
  DialogContent,
  Grid,
  IconButton,
  Button,
} from "@mui/material";
import { useEffect, useState } from "react";
import { GoPlus } from "react-icons/go";
import { MdClose } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { IIdName, IUser } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { UserForm } from "./UserForm";
import { UsersList } from "./UsersList";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const UsersListDialog = ({ isOpen, onClose }: IProps) => {
  const { authStore, generalStore } = useStore();
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAdd = () => {
    authStore.selectedUser = undefined;
    generalStore.showUserFormDialog = true;
  };

  return (
    <Dialog open={open} fullWidth maxWidth={"xs"}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        Users
      </BootstrapDialogTitle>
      <DialogContent>
        <UsersList />
        <Button
          onClick={handleAdd}
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
      </DialogContent>
    </Dialog>
  );
};
