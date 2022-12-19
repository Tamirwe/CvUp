import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { IInterviewer } from "../../models/AuthModels";
import { CrudTypes } from "../../models/GeneralEnums";
import { InterviewerForm } from "./InterviewerForm";

interface IProps {
  interviewer: IInterviewer;
  crudType?: CrudTypes;
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const InterviewerFormDialog = ({
  interviewer,
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
      case CrudTypes.Insert:
        setFormTitle("Add Team");
        break;
      case CrudTypes.Update:
        setFormTitle("Edit Team");
        break;
      case CrudTypes.Delete:
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
        <InterviewerForm
          interviewer={interviewer}
          crudType={crudType}
          onSaved={() => onClose(true)}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
