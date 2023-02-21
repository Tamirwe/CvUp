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
import { CustomerFormDialog } from "./CustomerFormDialog";
import { CustomersList } from "./CustomersList";

interface IProps {
  isOpen: boolean;
  close: () => void;
}

export const CustomersListDialog = ({ isOpen, close }: IProps) => {
  const { generalStore } = useStore();
  const [open, setOpen] = useState(false);
  const [openCustomerForm, setOpenCustomerForm] = useState(false);
  const [editCustomer, setEditCustomer] = useState<IIdName>();
  const [crudType, setCrudType] = useState<CrudTypesEnum>();

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAddEditDelete = (customer: IIdName, type: CrudTypesEnum) => {
    setEditCustomer(customer);
    setCrudType(type);
    setOpenCustomerForm(true);
  };

  const handleFormClose = (isSaved: boolean) => {
    setOpenCustomerForm(false);

    if (isSaved) {
      generalStore.getCustomersList(true);
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
        <CustomersList onAddEditDeleteclick={handleAddEditDelete} />
        <Button
          onClick={() =>
            handleAddEditDelete({ id: 0, name: "" }, CrudTypesEnum.Insert)
          }
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
        {openCustomerForm && editCustomer && (
          <CustomerFormDialog
            customer={editCustomer}
            crudType={crudType}
            isOpen={openCustomerForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
