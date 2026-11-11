import {
  Button,
  FormHelperText,
  IconButton,
  Grid,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { observer } from "mobx-react";
import { useRef, useState } from "react";
import { GoPlus } from "react-icons/go";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import {
  EmailTemplateTypeEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { IContractTemplate, IEmailTemplate } from "../../models/GeneralModels";
import { validateTxt, validteEmail } from "../../utils/Validation";
import { EmailTemplateFormDialog } from "../email/EmailTemplateFormDialog";
import { QuillRte } from "../rte/QuillRte";

interface IProps {
  template?: IContractTemplate;
  onSent: () => void;
}

const emptyForm = {
  companyName: "",
  companyIdNumber: "",
  companyAddress: "",
  contactName: "",
  emailAddress: "",
  emailSubject: "",
};

export const ContractSendForm = observer(({ template, onSent }: IProps) => {
  const { contractTemplatesStore, generalStore } = useStore();
  const refQuill = useRef();
  const [formModel, setFormModel] = useState({ ...emptyForm });
  const [submitError, setSubmitError] = useState("");
  const [bodyHtml, setBodyHtml] = useState("");
  const [isEmailTemplatesOpen, setIsEmailTemplatesOpen] = useState(false);
  const [updateFieldError, clearError, errModel] = useFormErrors({
    ...emptyForm,
  });

  const updateField = (field: keyof typeof emptyForm, value: string) => {
    setFormModel((currentProps) => ({ ...currentProps, [field]: value }));
    clearError(field);
    setSubmitError("");
  };

  const applyEmailTemplate = (emailTemplate: IEmailTemplate) => {
    updateField("emailSubject", emailTemplate.subject);
    setBodyHtml(emailTemplate.body);
  };

  const validateForm = () => {
    return [
      updateFieldError(
        "companyName",
        validateTxt(formModel.companyName, [TextValidateTypeEnum.notEmpty])
      ),
      updateFieldError(
        "emailAddress",
        validteEmail(formModel.emailAddress, [
          TextValidateTypeEnum.notEmpty,
          TextValidateTypeEnum.emailValid,
        ])
      ),
      updateFieldError(
        "emailSubject",
        validateTxt(formModel.emailSubject, [TextValidateTypeEnum.notEmpty])
      ),
    ].every((isValid) => isValid);
  };

  const sendContract = async () => {
    const quillEditor = refQuill.current as any;

    if (!validateForm()) {
      return;
    }

    if (!quillEditor.getText().trim()) {
      setSubmitError("Enter the email body");
      return;
    }

    if (!template) {
      setSubmitError("Select a contract first");
      return;
    }

    const res = await contractTemplatesStore.sendContract({
      contractTemplateId: template.id,
      companyName: formModel.companyName.trim(),
      companyIdNumber: formModel.companyIdNumber.trim(),
      companyAddress: formModel.companyAddress.trim(),
      contactName: formModel.contactName.trim(),
      emailAddress: formModel.emailAddress.trim(),
      emailSubject: formModel.emailSubject.trim(),
      emailBody: quillEditor.root.innerHTML,
    });

    if (res.isSuccess) {
      generalStore.alertSnackbar("success", "Contract sent");
      onSent();
    } else {
      generalStore.alertSnackbar("error", "Failed to send the contract");
    }
  };

  return (
    <form noValidate spellCheck="false">
      {isEmailTemplatesOpen && (
        <EmailTemplateFormDialog
          isOpen={isEmailTemplatesOpen}
          templateType={EmailTemplateTypeEnum.contract_email}
          onClose={() => setIsEmailTemplatesOpen(false)}
        />
      )}
      <Grid container spacing={2} sx={{ pt: 1 }}>
        <Grid item xs={12} md={4}>
          <TextField
            sx={{ direction: "rtl" }}
            fullWidth
            required
            id="contractCompanyName"
            label="Company name"
            variant="outlined"
            value={formModel.companyName}
            onChange={(e) => updateField("companyName", e.target.value)}
            error={errModel.companyName !== ""}
            helperText={errModel.companyName}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            fullWidth
            id="contractCompanyIdNumber"
            label="Company ID"
            variant="outlined"
            value={formModel.companyIdNumber}
            onChange={(e) => updateField("companyIdNumber", e.target.value)}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            sx={{ direction: "rtl" }}
            fullWidth
            id="contractCompanyAddress"
            label="Company address"
            variant="outlined"
            value={formModel.companyAddress}
            onChange={(e) => updateField("companyAddress", e.target.value)}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            sx={{ direction: "rtl" }}
            fullWidth
            id="contractContactName"
            label="Contact name"
            variant="outlined"
            value={formModel.contactName}
            onChange={(e) => updateField("contactName", e.target.value)}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            required
            type="email"
            id="contractEmailAddress"
            label="Email address"
            variant="outlined"
            value={formModel.emailAddress}
            onChange={(e) => updateField("emailAddress", e.target.value)}
            error={errModel.emailAddress !== ""}
            helperText={errModel.emailAddress}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ direction: "rtl" }}
            fullWidth
            required
            id="contractEmailSubject"
            label="Email subject"
            variant="outlined"
            value={formModel.emailSubject}
            onChange={(e) => updateField("emailSubject", e.target.value)}
            error={errModel.emailSubject !== ""}
            helperText={errModel.emailSubject}
          />
        </Grid>
        <Grid item xs={12} sx={{ direction: "rtl" }}>
          <Stack direction="row" flexWrap="wrap" alignItems="center" gap={1}>
            <IconButton
              color="primary"
              title="Edit email template"
              aria-label="Edit email template"
              onClick={() => setIsEmailTemplatesOpen(true)}
            >
              <GoPlus />
            </IconButton>
            {contractTemplatesStore.contractEmailTemplates.map((item) => (
              <Button
                key={item.id}
                size="small"
                variant="outlined"
                onClick={() => applyEmailTemplate(item)}
              >
                {item.name}
              </Button>
            ))}
          </Stack>
        </Grid>
        <Grid item xs={12}>
          <QuillRte ref={refQuill} quillHtml={bodyHtml} />
        </Grid>
        <Grid item xs={12}>
          <FormHelperText error>{submitError}</FormHelperText>
          <Stack
            direction="row"
            alignItems="center"
            justifyContent="space-between"
            gap={2}
          >
            <Typography variant="body2" color="text.secondary" noWrap>
              {template
                ? `Contract: ${template.name}`
                : "Select a contract first"}
            </Typography>
            <Button
              variant="contained"
              color="secondary"
              onClick={sendContract}
              sx={{ flexShrink: 0 }}
            >
              Send
            </Button>
          </Stack>
        </Grid>
      </Grid>
    </form>
  );
});
