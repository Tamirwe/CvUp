import {
  Dialog,
  DialogTitle,
  DialogContent,
  IconButton,
  Button,
} from "@mui/material";
import { useEffect, useState } from "react";
import { GoPlus } from "react-icons/go";
import { MdClose } from "react-icons/md";
import { IIdName } from "../../models/AuthModels";
import { DepartmentFormDialog } from "./DepartmentFormDialog";
import { DepartmentsList } from "./DepartmentsList";

interface IProps {
  isOpen: boolean;
  close: () => void;
}

export const DepartmentListDialog = ({ isOpen, close }: IProps) => {
  const [open, setOpen] = useState(false);
  const [openDepartmentForm, setOpenDepartmentForm] = useState(false);
  const [editDepartment, setEditDepartment] = useState<IIdName>();

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAddEdit = (department: IIdName) => {
    setEditDepartment(department);
    setOpenDepartmentForm(true);
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth={"xs"}>
      <DialogTitle>
        Company Teams{" "}
        <IconButton
          aria-label="close"
          onClick={close}
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
        <DepartmentsList onAddEdit={handleAddEdit} />
        <Button
          onClick={() => handleAddEdit({ id: 0, name: "" })}
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          New Department
        </Button>
        {openDepartmentForm && (
          <DepartmentFormDialog
            department={editDepartment}
            isOpen={openDepartmentForm}
            onClose={() => setOpenDepartmentForm(false)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
