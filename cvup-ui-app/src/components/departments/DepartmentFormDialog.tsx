import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { IIdName } from "../../models/AuthModels";
import { CrudTypes } from "../../models/GeneralEnums";
import { DepartmentForm } from "./DepartmentForm";

interface IProps {
  department: IIdName;
  crudType?: CrudTypes;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const DepartmentFormDialog = ({
  department,
  crudType,
  isOpen,
  onClose,
}: IProps) => {
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("");

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  useEffect(() => {
    switch (crudType) {
      case CrudTypes.Insert:
        setFormTitle("Add Team");
        break;
      case CrudTypes.Update:
        setFormTitle("Edit Team");
        break;
      case CrudTypes.Delete:
        setFormTitle("Delete Team");
        break;
      default:
        break;
    }
  }, [crudType]);

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"xs"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <DepartmentForm
          department={department}
          crudType={crudType}
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
