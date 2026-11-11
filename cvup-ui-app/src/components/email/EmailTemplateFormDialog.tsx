import { Dialog, DialogContent } from "@mui/material";
import { useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { EmailTemplateTypeEnum } from "../../models/GeneralEnums";
import { EmailTemplateForm } from "./EmailTemplateForm";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
  templateType?: EmailTemplateTypeEnum;
}

export const EmailTemplateFormDialog = ({
  isOpen,
  onClose,
  templateType = EmailTemplateTypeEnum.cand_email,
}: IProps) => {
  const { candsStore, contractTemplatesStore } = useStore();
  const [formTitle] = useState(
    templateType === EmailTemplateTypeEnum.contract_email
      ? "Contract Email Templates"
      : "Email Templates"
  );

  return (
    <Dialog open={isOpen} fullWidth maxWidth={"md"}>
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        {formTitle}
      </BootstrapDialogTitle>
      <DialogContent>
        <EmailTemplateForm
          templateType={templateType}
          onSaved={() => {
            if (templateType === EmailTemplateTypeEnum.contract_email) {
              contractTemplatesStore.getContractEmailTemplates();
            } else {
              candsStore.getEmailTemplates();
            }
            onClose(true);
          }}
          onCancel={() => onClose(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
