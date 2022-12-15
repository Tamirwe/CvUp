import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { IIdName } from "../../models/AuthModels";
import { DepartmentForm } from "./DepartmentForm";

interface IProps {
  department?: IIdName;
  isOpen: boolean;
  onClose: () => void;
}

export const DepartmentFormDialog = ({
  department,
  isOpen,
  onClose,
}: IProps) => {
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add New Department / Team");

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  useEffect(() => {
    if (department && department.id > 0) {
      setFormTitle("Edit Department / Team");
    }
  }, [department]);

  return (
    <Dialog open={open} onClose={() => onClose()} fullWidth maxWidth={"xs"}>
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <DepartmentForm department={department} onClose={() => onClose()} />
      </DialogContent>
    </Dialog>
  );
};
