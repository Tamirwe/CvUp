import {
  Autocomplete,
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  Grid,
  Stack,
  TextField,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IEmailForm, IMailsList } from "../../models/GeneralModels";
import { emailValidte, isEmailValid } from "../../utils/Validation";
import { QuillRte } from "../rte/QuillRte";
import { EmailsToControl } from "./EmailsToControl";
import { useFormErrors } from "../../Hooks/useFormErrors";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const CandidateEmailSender = (props: IProps) => {
  const { candsStore } = useStore();
  const [quillEditor, setQuillEditor] = useState<any>(null);
  const [emailsToList, setEmailsToList] = useState<IMailsList[]>([]);
  const [listDefaultEmails, setListDefaultEmails] = useState<IMailsList[]>([]);
  const [reviewHtml, setReviewHtml] = useState("");
  const [formModel, setFormModel] = useState<IEmailForm>({
    subject: "",
    review: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    subject: "",
  });

  useEffect(() => {
    if (candsStore.candDisplay) {
      const emailsList = [
        {
          email: candsStore.candAllSelected?.email || "",
          name: candsStore.candAllSelected?.candidateName || "",
        },
      ];

      var reviewLines = candsStore.candDisplay.review?.split("\n");

      const reviewLinesHtml = reviewLines?.map((item, i) => {
        return `<p style="text-align: right; direction: rtl;">${item}</p>`;
      });

      setReviewHtml(reviewLinesHtml?.join("") || "");
      setEmailsToList(emailsList);
      setListDefaultEmails(emailsList);
    }
  }, []);

  const handleRteInit = (editor: any) => {
    setQuillEditor(editor);
  };

  const handleSend = async () => {
    const reviewText = quillEditor.getText();
    const reviewHtml = quillEditor.root.innerHTML;
    // <p style="text-align: right; direction: rtl;">טל פון בישראל 0515405763</p>
    // <p style="text-align: right; direction: rtl;">נמצא בבוסטון עם אשתו - נמצאת בהרווארד</p>
    //await candsStore.saveCvReview(reviewText, reviewHtml);
    props.onClose();
  };

  return (
    <Dialog open={props.open} onClose={props.onClose} fullWidth maxWidth="md">
      <DialogTitle>Send Email To Candidate</DialogTitle>
      <DialogContent>
        <Grid item xs={12} lg={12}>
          <TextField
            sx={{
              direction: "rtl",
            }}
            fullWidth
            required
            margin="normal"
            type="text"
            id="Subject"
            label="Subject"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                name: e.target.value,
              }));
              clearError("subject");
            }}
            error={errModel.subject !== ""}
            helperText={errModel.subject}
            value={formModel.subject}
          />
        </Grid>
        <EmailsToControl
          listEmailsTo={emailsToList}
          listDefaultEmails={listDefaultEmails}
        />
        <QuillRte onInit={handleRteInit} quillHtml={reviewHtml} />
        <Grid container>
          <Grid item xs={12} lg={12} pt={2}>
            <Stack direction="row" justifyContent="flex-end" gap={1}>
              <Button color="primary" onClick={handleSend}>
                Send
              </Button>
              <Button color="secondary" onClick={() => props.onClose()}>
                Cancel
              </Button>
            </Stack>
          </Grid>
        </Grid>
      </DialogContent>
    </Dialog>
  );
};
