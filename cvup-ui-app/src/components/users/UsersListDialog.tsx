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
    <Dialog open={open} onClose={onClose} fullWidth maxWidth={"xs"}>
      <DialogTitle>
        Users{" "}
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{
            position: "absolute",
            right: 8,
            top: 8,
            color: (theme) => theme.palette.grey[500],
          }}
        >
          <MdClose />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <UsersList />
        <Button
          onClick={handleAdd}
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>

        {/* {openInterviewerForm && editInterviewer && (
          <InterviewerFormDialog
            interviewer={editInterviewer}
            crudType={crudType}
            isOpen={openInterviewerForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )} */}
      </DialogContent>
    </Dialog>
  );
};
