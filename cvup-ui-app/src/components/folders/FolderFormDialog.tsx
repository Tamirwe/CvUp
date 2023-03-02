import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IFolder } from "../../models/GeneralModels";
import { FolderForm } from "./FolderForm";
import { UpdateDeleteFolderForm } from "./UpdateDeleteFolderForm";

interface IProps {
  folder?: IFolder;
  crudType?: CrudTypesEnum;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const FolderFormDialog = ({
  // folder,
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
        setFormTitle("Add Folder");
        break;
      case CrudTypesEnum.Update:
        setFormTitle("Edit Folder");
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
        <UpdateDeleteFolderForm
          crudType={crudType}
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
        <FolderForm
          // folder={folder}
          crudType={crudType}
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
