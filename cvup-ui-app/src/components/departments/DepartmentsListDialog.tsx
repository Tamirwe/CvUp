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
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { DepartmentFormDialog } from "./DepartmentFormDialog";
import { DepartmentsList } from "./DepartmentsList";

interface IProps {
  isOpen: boolean;
  close: () => void;
}

export const DepartmentsListDialog = ({ isOpen, close }: IProps) => {
  const { generalStore } = useStore();
  const [open, setOpen] = useState(false);
  const [openDepartmentForm, setOpenDepartmentForm] = useState(false);
  const [editDepartment, setEditDepartment] = useState<IIdName>();
  const [crudType, setCrudType] = useState<CrudTypesEnum>();

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAddEditDelete = (department: IIdName, type: CrudTypesEnum) => {
    setEditDepartment(department);
    setCrudType(type);
    setOpenDepartmentForm(true);
  };

  const handleFormClose = (isSaved: boolean) => {
    setOpenDepartmentForm(false);

    if (isSaved) {
      generalStore.getDepartmentsList(true);
    }
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
        <DepartmentsList onAddEditDeleteclick={handleAddEditDelete} />
        <Button
          onClick={() =>
            handleAddEditDelete({ id: 0, name: "" }, CrudTypesEnum.Insert)
          }
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
        {openDepartmentForm && editDepartment && (
          <DepartmentFormDialog
            department={editDepartment}
            crudType={crudType}
            isOpen={openDepartmentForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
