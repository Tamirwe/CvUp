import {
  Dialog,
  DialogTitle,
  DialogContent,
  IconButton,
  Button,
  Grid,
  TextField,
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
  onClose: () => void;
}

export const CustomersListDialog = ({ isOpen, onClose }: IProps) => {
  const { customersContactsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [openCustomerForm, setOpenCustomerForm] = useState(false);
  const [searchTxt, setSearchTxt] = useState("");
  const [dataList, setDataList] = useState<IIdName[]>([]);

  useEffect(() => {
    setDataList([...customersContactsStore.customersList]);
  }, []);

  useEffect(() => {
    let fDataList;

    if (searchTxt) {
      fDataList = customersContactsStore.customersList.filter(
        (x) => x.name.toLowerCase().indexOf(searchTxt) > -1
      );
    } else {
      fDataList = [...customersContactsStore.customersList];
    }

    setDataList(fDataList);
  }, [searchTxt]);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleCustomerClick = () => {
    setOpenCustomerForm(true);
  };

  const handleFormClose = (isSaved: boolean) => {
    setOpenCustomerForm(false);

    if (isSaved) {
      customersContactsStore.getCustomersList();
    }

    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth={"xs"}>
      <DialogTitle>
        Customers{" "}
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
        <Grid container>
          <Grid item xs={12}>
            <TextField
              fullWidth
              required
              margin="normal"
              type="text"
              id="search"
              label="Search Customer"
              variant="outlined"
              onChange={(e) => {
                setSearchTxt(e.target.value);
              }}
              value={searchTxt}
            />
          </Grid>
        </Grid>
        <CustomersList
          onCustomerClick={handleCustomerClick}
          dataList={dataList}
        />
        <Button
          onClick={() => {
            customersContactsStore.selectedCustomer = undefined;
            handleCustomerClick();
          }}
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
        {openCustomerForm && (
          <CustomerFormDialog
            isOpen={openCustomerForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
