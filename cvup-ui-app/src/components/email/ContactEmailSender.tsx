import { Dialog, DialogContent, DialogTitle } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IMailsList } from "../../models/GeneralModels";
import { QuillRte } from "../rte/QuillRte";
import { EmailsToControl } from "./EmailsToControl";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const ContactEmailSender = (props: IProps) => {
  const { candsStore } = useStore();
  const [quillEditor, setQuillEditor] = useState<any>(null);
  const [emailsToList, setEmailsToList] = useState<IMailsList[]>([]);
  const [listDefaultEmails, setListDefaultEmails] = useState<IMailsList[]>([]);

  useEffect(() => {
    if (candsStore.candAllSelected) {
      const emailsList = [
        {
          email: candsStore.candAllSelected?.email || "",
          name: candsStore.candAllSelected?.candidateName || "",
        },
      ];

      setEmailsToList(emailsList);
      setListDefaultEmails(emailsList);
    }
  }, []);

  const handleRteInit = (editor: any) => {
    setQuillEditor(editor);
  };

  return (
    <Dialog open={props.open} onClose={props.onClose} fullWidth maxWidth="md">
      <DialogTitle>Send Email To Contact</DialogTitle>
      <DialogContent>
        <EmailsToControl
          listEmailsTo={emailsToList}
          listDefaultEmails={listDefaultEmails}
        />
        <QuillRte
          onInit={handleRteInit}
          quillHtml={candsStore.candAllSelected?.review}
        />
      </DialogContent>
    </Dialog>
  );
};
