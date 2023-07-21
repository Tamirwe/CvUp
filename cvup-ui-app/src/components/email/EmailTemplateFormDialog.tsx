import { Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { EmailTemplateForm } from "./EmailTemplateForm";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const EmailTemplateFormDialog = ({ isOpen, onClose }: IProps) => {
  const { candsStore } = useStore();
  const [formTitle, setFormTitle] = useState("Email Templates");

  return (
    <Dialog
      open={isOpen}
      onClose={() => onClose(false)}
      fullWidth
      maxWidth={"sm"}
    >
      <DialogTitle>{formTitle}</DialogTitle>
      <DialogContent>
        <EmailTemplateForm
          onSaved={() => {
            candsStore.getEmailTemplates();
            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
