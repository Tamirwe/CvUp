import {
  Autocomplete,
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  FormControl,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IEmailForm, IMailsList } from "../../models/GeneralModels";
import {
  emailValidte,
  isEmailValid,
  validateTxt,
} from "../../utils/Validation";
import { QuillRte } from "../rte/QuillRte";
import { EmailsToControl } from "./EmailsToControl";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { isMobile } from "react-device-detect";
import {
  CrudTypesEnum,
  PositionStatusEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { CiEdit } from "react-icons/ci";
import { observer } from "mobx-react-lite";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const CandidateEmailSender = observer((props: IProps) => {
  const { candsStore, generalStore } = useStore();
  const refQuill = useRef();
  const [emailTemplate, setEmailTemplate] = useState(
    candsStore.emailTemplates ? candsStore.emailTemplates[0] : null
  );
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
          name:
            (candsStore.candDisplay?.firstName || "") +
            " " +
            (candsStore.candDisplay?.lastName || ""),
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
    // setQuillEditor(editor);
    // ref.current = editor;
  };

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.subject, [TextValidateTypeEnum.notEmpty]);

    isFormValid = updateFieldError("subject", err) && isFormValid;

    return isFormValid;
  };

  const handleSend = async () => {
    const quillEditor = refQuill.current as any;
    const reviewText = quillEditor.getText();
    const reviewHtml = quillEditor.root.innerHTML;
    // <p style="text-align: right; direction: rtl;">טל פון בישראל 0515405763</p>
    // <p style="text-align: right; direction: rtl;">נמצא בבוסטון עם אשתו - נמצאת בהרווארד</p>
    //await candsStore.saveCvReview(reviewText, reviewHtml);
    props.onClose();
  };

  return (
    <Dialog
      fullScreen={isMobile ? true : false}
      open={props.open}
      onClose={props.onClose}
      fullWidth
      maxWidth="md"
    >
      <DialogTitle>Send Email To Candidate</DialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={12} lg={12} pt={1}>
            <EmailsToControl
              listEmailsTo={emailsToList}
              listDefaultEmails={listDefaultEmails}
            />
          </Grid>
          <Grid item xs={12} lg={8} pt={1}>
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
                  name: e.target.value,
                }));
                clearError("subject");
              }}
              error={errModel.subject !== ""}
              helperText={errModel.subject}
              value={formModel.subject}
            />
          </Grid>
          <Grid item xs={12} lg={4} pt={1}>
            <Stack direction="row">
              <FormControl fullWidth>
                <InputLabel id="emailTemplatelabel">Email Template</InputLabel>
                <Select
                  sx={{ direction: "ltr" }}
                  labelId="emailTemplate"
                  id="emailTemplate"
                  label="Email Template"
                  onChange={(e) => {
                    setFormModel((currentProps) => ({
                      ...currentProps,
                      status: e.target.value as PositionStatusEnum,
                    }));
                  }}
                  value={emailTemplate && emailTemplate.id}
                >
                  {candsStore.emailTemplates?.map((item) => {
                    return (
                      <MenuItem key={item.id} value={item.id}>
                        {item.name}
                      </MenuItem>
                    );
                  })}
                </Select>
              </FormControl>
              <IconButton
                color="primary"
                onClick={() => {
                  generalStore.showEmailTemplatesDialog = true;
                }}
              >
                <CiEdit />
              </IconButton>
            </Stack>
          </Grid>
        </Grid>
        <QuillRte
          onInit={handleRteInit}
          ref={refQuill}
          quillHtml={reviewHtml}
        />
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
});
