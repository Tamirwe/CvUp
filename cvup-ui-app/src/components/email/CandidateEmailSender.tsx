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
import {
  IEmailForm,
  IEmailTemplate,
  IEmailsAddress,
  ISendEmail,
} from "../../models/GeneralModels";
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
  DynamicEmailDataEnum,
  PositionStatusEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { CiEdit } from "react-icons/ci";
import { observer } from "mobx-react-lite";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  open: boolean;
  onClose: () => void;
}

export const CandidateEmailSender = observer((props: IProps) => {
  const { candsStore, generalStore, positionsStore, authStore } = useStore();
  const refQuill = useRef();
  const [dynamicData, setDynamicData] = useState<Map<string, string>>();

  const [emailTemplate, setEmailTemplate] = useState<IEmailTemplate>({
    id: 0,
    body: "",
    name: "",
    subject: "",
  });
  const [emailsOptionsList, setEmailsOptionsList] = useState<IEmailsAddress[]>(
    []
  );
  const [toEmailsList, setToEmailsList] = useState<IEmailsAddress[]>([]);
  const [bodyHtml, setBodyHtml] = useState("");
  const [formModel, setFormModel] = useState<IEmailForm>({
    subject: "",
    body: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    subject: "",
    body: "",
  });

  const addCandidateEmail = () => {
    if (candsStore.candDisplay) {
      const candName = `${candsStore.candDisplay?.firstName?.trim() || ""} ${
        candsStore.candDisplay?.lastName?.trim() || ""
      }`;

      const emailsList = [
        {
          id: 1,
          Address: candsStore.candDisplay?.email || "",
          Name: candName.trim() ? candName : candsStore.candDisplay?.email,
        },
      ];

      setEmailsOptionsList(emailsList);
      setToEmailsList(emailsList);
    }
  };

  const generateDynamicDataForEmailTemplate = () => {
    if (candsStore.candDisplay) {
      const dynamicDataMap = new Map();

      dynamicDataMap.set("[FirstName]", candsStore.candDisplay.firstName);
      dynamicDataMap.set(
        "[FullName]",
        `${candsStore.candDisplay.firstName} ${candsStore.candDisplay.lastName}`
      );

      dynamicDataMap.set(
        "[CustomerName]",
        positionsStore.candDisplayPosition?.customerName
      );

      dynamicDataMap.set(
        "[PositionName]",
        positionsStore.candDisplayPosition?.name
      );

      dynamicDataMap.set("[MyFirstName]", authStore.currentUser?.firstName);

      dynamicDataMap.set(
        "[MyFullname]",
        `${authStore.currentUser?.firstName} ${authStore.currentUser?.lastName}`
      );

      dynamicDataMap.set("[MyFirstNameEn]", authStore.currentUser?.firstNameEn);

      dynamicDataMap.set(
        "[MyFullnameEn]",
        `${authStore.currentUser?.firstNameEn} ${authStore.currentUser?.lastNameEn}`
      );

      dynamicDataMap.set("[MyEmail]", authStore.currentUser?.email);

      dynamicDataMap.set("[MyPhoneNumber]", authStore.currentUser?.phone);

      setDynamicData(dynamicDataMap);
    }
  };

  useEffect(() => {
    addCandidateEmail();
    generateDynamicDataForEmailTemplate();
  }, []);

  useEffect(() => {
    let subject = replaceDynamicData(emailTemplate.subject);
    let body = replaceDynamicData(emailTemplate.body);

    setFormModel((currentProps) => ({
      ...currentProps,
      subject: subject,
    }));

    setBodyHtml(body);
  }, [emailTemplate]);

  const replaceDynamicData = (str: string) => {
    let strReplaced = str;

    Object.keys(DynamicEmailDataEnum)
      .filter((x) => isNaN(Number(x)))
      .forEach((item) => {
        strReplaced = strReplaced.replaceAll(
          `[${item}]`,
          dynamicData?.get(`[${item}]`) || ""
        );
      });

    return strReplaced;
  };

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

      const emailData: ISendEmail = {
        toAddresses: toEmailsList,
        subject: formModel.subject,
        body: emailBody,
      };

      var data = await candsStore.sendEmailToCandidate(
        emailData,
        emailTemplate
      );

      if (data.isSuccess) {
        generalStore.alertSnackbar("success", "Email sent");
        props.onClose();
      } else {
        generalStore.alertSnackbar("error", "Email send error - check outbox");
      }
    }
  };

  return (
    <Dialog
      fullScreen={isMobile ? true : false}
      open={props.open}
      // onClose={props.onClose}
      fullWidth
      maxWidth="md"
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => props.onClose()}>
        Send Email To Candidate
      </BootstrapDialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={12} lg={12} pt={1}>
            <EmailsToControl
              optionsList={emailsOptionsList}
              listValues={toEmailsList}
              onChange={(vals) => {
                setToEmailsList(vals);
              }}
            />
          </Grid>
          <Grid item xs={12} lg={8} pt={2}>
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
          <Grid item xs={12} lg={4} pt={2}>
            <Stack direction="row">
              <FormControl fullWidth sx={{ direction: "rtl" }}>
                <InputLabel id="emailTemplatelabel">Email Template</InputLabel>
                <Select
                  labelId="emailTemplate"
                  id="emailTemplate"
                  label="Email Template"
                  onChange={(e) => {
                    if (e.target.value) {
                      const emTemplate = candsStore.emailTemplates?.find(
                        (x) => x.id === e.target.value
                      );
                      emTemplate && setEmailTemplate(emTemplate);
                    } else {
                      setEmailTemplate({
                        id: 0,
                        body: "",
                        name: "",
                        subject: "",
                      });
                    }
                  }}
                  value={emailTemplate?.id}
                >
                  <MenuItem key={0} value={0} sx={{ direction: "rtl" }}>
                    &nbsp;
                  </MenuItem>
                  {candsStore.emailTemplates?.map((item) => {
                    return (
                      <MenuItem
                        key={item.id}
                        value={item.id}
                        sx={{ direction: "rtl" }}
                      >
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
});
