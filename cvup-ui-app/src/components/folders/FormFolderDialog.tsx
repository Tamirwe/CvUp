import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IFolder } from "../../models/GeneralModels";
import { FormAddChild } from "./FormAddChild";
import { FormUpdateDelete } from "./FormUpdateDelete";

interface IProps {
  folder?: IFolder;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const FormFolderDialog = ({
  // folder,
  isOpen,
  onClose,
}: IProps) => {
  const { generalStore } = useStore();
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

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"xs"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        {generalStore.openModeFolderFormDialog === CrudTypesEnum.Update ? (
          <FormUpdateDelete
            onAddChild={() =>
              (generalStore.openModeFolderFormDialog = CrudTypesEnum.Insert)
            }
            onSaved={() => onClose(true)}
            onCancel={() => onClose(false)}
          />
        ) : (
          <FormAddChild
            onSaved={() => onClose(true)}
            onCancel={() => onClose(false)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
