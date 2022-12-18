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
import { CrudTypes } from "../../models/GeneralEnums";
import { HrCompanyFormDialog } from "./HrCompanyFormDialog";
import { HrCompaniesList } from "./HrCompaniesList";

interface IProps {
  isOpen: boolean;
  close: () => void;
}

export const HrCompaniesListDialog = ({ isOpen, close }: IProps) => {
  const { generalStore } = useStore();
  const [open, setOpen] = useState(false);
  const [openhrCompanyForm, setOpenhrCompanyForm] = useState(false);
  const [edithrCompany, setEdithrCompany] = useState<IIdName>();
  const [crudType, setCrudType] = useState<CrudTypes>();

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAddEditDelete = (hrCompany: IIdName, type: CrudTypes) => {
    setEdithrCompany(hrCompany);
    setCrudType(type);
    setOpenhrCompanyForm(true);
  };

  const handleFormClose = (isSaved: boolean) => {
    setOpenhrCompanyForm(false);

    if (isSaved) {
      generalStore.getHrCompanies(true);
    }
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth={"xs"}>
      <DialogTitle>
        HR Companies{" "}
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
        <HrCompaniesList onAddEditDeleteclick={handleAddEditDelete} />
        <Button
          onClick={() =>
            handleAddEditDelete({ id: 0, name: "" }, CrudTypes.Insert)
          }
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
        {openhrCompanyForm && (
          <HrCompanyFormDialog
            hrCompany={edithrCompany}
            crudType={crudType}
            isOpen={openhrCompanyForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
