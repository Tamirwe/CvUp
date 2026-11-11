import { Dialog, DialogContent } from "@mui/material";
import { IContractTemplate } from "../../models/GeneralModels";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { ContractSendForm } from "./ContractSendForm";

interface IProps {
  isOpen: boolean;
  template?: IContractTemplate;
  onClose: () => void;
}

export const ContractSendDialog = ({ isOpen, template, onClose }: IProps) => {
  return (
    <Dialog open={isOpen} onClose={onClose} fullWidth maxWidth={"md"}>
      <BootstrapDialogTitle id="contract-send-title" onClose={onClose}>
        Send Contract Email
      </BootstrapDialogTitle>
      <DialogContent>
        <ContractSendForm template={template} onSent={onClose} />
      </DialogContent>
    </Dialog>
  );
};
