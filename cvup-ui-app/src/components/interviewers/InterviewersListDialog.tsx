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
import { IInterviewer } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { InterviewerFormDialog } from "./InterviewerFormDialog";
import { InterviewersList } from "./InterviewersList";

interface IProps {
  isOpen: boolean;
  close: () => void;
}

export const InterviewersListDialog = ({ isOpen, close }: IProps) => {
  const { authStore } = useStore();
  const [open, setOpen] = useState(false);
  const [openInterviewerForm, setOpenInterviewerForm] = useState(false);
  const [editInterviewer, setEditInterviewer] = useState<IInterviewer>();
  const [crudType, setCrudType] = useState<CrudTypesEnum>();

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleAddEditDelete = (
    interviewer: IInterviewer,
    type: CrudTypesEnum
  ) => {
    setEditInterviewer(interviewer);
    setCrudType(type);
    setOpenInterviewerForm(true);
  };

  const handleFormClose = (isSaved: boolean) => {
    setOpenInterviewerForm(false);

    if (isSaved) {
      authStore.getInterviewersList(true);
    }
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth={"xs"}>
      <DialogTitle>
        Interviewers{" "}
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
        <InterviewersList onAddEditDeleteclick={handleAddEditDelete} />
        <Button
          onClick={() =>
            handleAddEditDelete(
              {
                id: 0,
                firstName: "",
                lastName: "",
                email: "",
                permissionType: 20,
              },
              CrudTypesEnum.Insert
            )
          }
          sx={{ padding: "30px 0 10px 0" }}
          startIcon={<GoPlus />}
        >
          Add
        </Button>
        {openInterviewerForm && editInterviewer && (
          <InterviewerFormDialog
            interviewer={editInterviewer}
            crudType={crudType}
            isOpen={openInterviewerForm}
            onClose={(isSaved) => handleFormClose(isSaved)}
          />
        )}
      </DialogContent>
    </Dialog>
  );
};
