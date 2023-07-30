import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IFolder } from "../../models/GeneralModels";
import { FormAddChild } from "./FormAddChild";
import { FolderForm } from "./FolderForm";

interface IProps {
  folder?: IFolder;
  isOpen: boolean;
  onClose: () => void;
}

export const FolderFormDialog = ({
  // folder,
  isOpen,
  onClose,
}: IProps) => {
  const { foldersStore, generalStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("");

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  useEffect(() => {
    switch (generalStore.openModeFolderFormDialog) {
      case CrudTypesEnum.Insert:
        setFormTitle("Add Folder");
        break;
      case CrudTypesEnum.Update:
        setFormTitle("Edit Folder");
        break;
      default:
        break;
    }
  }, [generalStore.openModeFolderFormDialog]);

  const handleSaved = () => {
    onClose();
    foldersStore.getFoldersList();
  };

  return (
    <Dialog open={open} onClose={() => onClose()} fullWidth maxWidth={"xs"}>
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        {generalStore.openModeFolderFormDialog === CrudTypesEnum.Update ? (
          <FolderForm
            onAddFolderChild={() =>
              (generalStore.openModeFolderFormDialog = CrudTypesEnum.Insert)
            }
            onSaved={handleSaved}
            onCancel={() => onClose()}
          />
        ) : (
          <FormAddChild onSaved={handleSaved} onCancel={() => onClose()} />
        )}
      </DialogContent>
    </Dialog>
  );
};
