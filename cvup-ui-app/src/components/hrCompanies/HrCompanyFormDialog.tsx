import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { HrCompanyForm } from "./HrCompanyForm";

interface IProps {
  hrCompany: IIdName;
  crudType?: CrudTypesEnum;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const HrCompanyFormDialog = ({
  hrCompany,
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
      case CrudTypesEnum.Insert:
        setFormTitle("Add Team");
        break;
      case CrudTypesEnum.Update:
        setFormTitle("Edit Team");
        break;
      case CrudTypesEnum.Delete:
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
        <HrCompanyForm
          hrCompany={hrCompany}
          crudType={crudType}
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
