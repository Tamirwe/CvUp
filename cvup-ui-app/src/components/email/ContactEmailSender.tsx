import {
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  Grid,
  Stack,
  TextField,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import {
  IAttachCv,
  IEmailForm,
  IEmailsAddress,
  ISendEmail,
} from "../../models/GeneralModels";
import { QuillRte } from "../rte/QuillRte";
import { EmailsToControl } from "./EmailsToControl";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { validateTxt } from "../../utils/Validation";
import { TextValidateTypeEnum } from "../../models/GeneralEnums";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const ContactEmailSender = (props: IProps) => {
  const { candsStore, generalStore, customersContactsStore } = useStore();
  const refQuill = useRef();
  const [bodyHtml, setBodyHtml] = useState("");

  const [emailsToList, setEmailsToList] = useState<IEmailsAddress[]>([]);
  const [listDefaultEmails, setListDefaultEmails] = useState<IEmailsAddress[]>(
    []
  );
  const [formModel, setFormModel] = useState<IEmailForm>({
    subject: "",
    body: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    subject: "",
    body: "",
  });

  useEffect(() => {
    if (candsStore.candDisplay) {
      var reviewLines = candsStore.candDisplay.review?.split("\n");

      const reviewLinesHtml = reviewLines?.map((item, i) => {
        return `<p style="text-align: right; direction: rtl;">${item}</p>`;
      });

      setBodyHtml(reviewLinesHtml?.join("") || "");

      // const emailsList = candsStore.candDisplay.position?.contactsIds.map(
      //   (id) => {
      //     const contact = customersContactsStore.getContactById(id);
      //     return {
      //       Address: contact.email || "",
      //       Name: (contact.firstName || "") + " " + (contact.lastName || ""),
      //     };
      //   }
      // );

      // if (emailsList) {
      //   setEmailsToList(emailsList);
      //   setListDefaultEmails(emailsList);
      // }
    }
  }, []);

  const validateForm = () => {
    const quillEditor = refQuill.current as any;
    const body = quillEditor.getText();

    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.subject, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
    ]);
    isFormValid = updateFieldError("subject", err) && isFormValid;

    err = validateTxt(body, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
    ]);
    isFormValid = updateFieldError("body", err) && isFormValid;

    return isFormValid;
  };

  const handleSend = async () => {
    const isValid = validateForm();

    if (isValid) {
      const quillEditor = refQuill.current as any;
      const emailBody = quillEditor.root.innerHTML;
      const attachment: IAttachCv = {
        cvKey: candsStore.candDisplay?.keyId || "",
        name: `${candsStore.candDisplay?.firstName} ${candsStore.candDisplay?.lastName}`,
      };

      const emailData: ISendEmail = {
        toAddresses: emailsToList,
        subject: formModel.subject,
        body: emailBody,
        attachCvs: [attachment],
      };

      var data = await candsStore.sendEmail(emailData);

      if (data.isSuccess) {
        generalStore.alertSnackbar("success", "Email sent");
      } else {
        generalStore.alertSnackbar("error", "Email send error - check outbox");
      }

      props.onClose();
    }
  };

  return (
    <Dialog open={props.open} onClose={props.onClose} fullWidth maxWidth="md">
      <DialogTitle>Send Email To Contact</DialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={12} lg={12} pt={1}>
            <EmailsToControl
              listEmailsTo={emailsToList}
              listDefaultEmails={listDefaultEmails}
            />
          </Grid>
          <Grid item xs={12} lg={12} pt={2}>
            <TextField
              sx={{
                mt: 0,
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
                  subject: e.target.value,
                }));
                clearError("subject");
              }}
              error={errModel.subject !== ""}
              helperText={errModel.subject}
              value={formModel.subject}
            />
          </Grid>
          <Grid item xs={12} lg={12} pt={1}>
            <QuillRte ref={refQuill} quillHtml={bodyHtml} />
          </Grid>
          <Grid item xs={12} lg={12} pt={1}>
            <div style={{ color: "#f44336" }}>{errModel.body}</div>
          </Grid>
        </Grid>
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
