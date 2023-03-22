import { Dialog, DialogTitle, DialogContent, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { UserForm } from "./UserForm";
import { UsersListWrapper } from "./UsersListWrapper";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const UsersDialog = ({ isOpen, onClose }: IProps) => {
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("");

  useEffect(() => {
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
        <Grid container>
          <Grid item xs={4}>
            <UsersListWrapper />
          </Grid>
          <Grid item xs={8}>
            <UserForm
              onSaved={() => onClose(true)}
              onCancel={() => onClose(false)}
            />
          </Grid>
        </Grid>
      </DialogContent>
    </Dialog>
  );
};
