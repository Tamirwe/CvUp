import { Dialog, DialogContent, DialogTitle } from "@mui/material";
import { useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { QuillRte } from "../rte/QuillRte";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const CandidateEmailSender = (props: IProps) => {
  const { candsStore } = useStore();

  const [quillEditor, setQuillEditor] = useState<any>(null);

  const handleRteInit = (editor: any) => {
    setQuillEditor(editor);
  };

  return (
    <Dialog open={props.open} onClose={props.onClose} fullWidth maxWidth="md">
      <DialogTitle>Send Email To Candidate</DialogTitle>
      <DialogContent>
        <QuillRte
          onInit={handleRteInit}
          quillHtml={candsStore.candSelected?.review}
        />
      </DialogContent>
    </Dialog>
  );
};
