import {
  Box,
  Button,
  FormHelperText,
  Grid,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { observer } from "mobx-react";
import { useState } from "react";
import { GoPlus } from "react-icons/go";
import { MdDelete, MdEdit } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { useFormErrors } from "../../Hooks/useFormErrors";
import {
  AlertConfirmDialogEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { IContact } from "../../models/GeneralModels";
import {
  validateTxt,
  validteEmail,
  validtePhone,
} from "../../utils/Validation";

interface IProps {
  customerId: number;
  customerName: string;
}

const emptyContact: IContact = {
  id: 0,
  customerId: 0,
  customerName: "",
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  role: "",
};

const emptyErrors = {
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
};

export const CustomerContacts = observer(
  ({ customerId, customerName }: IProps) => {
    const { customersContactsStore, generalStore } = useStore();
    const [formModel, setFormModel] = useState<IContact | undefined>();
    const [isDirty, setIsDirty] = useState(false);
    const [submitError, setSubmitError] = useState("");
    const [updateFieldError, clearError, errModel] = useFormErrors(emptyErrors);

    const contacts = customersContactsStore
      .getContactsByCustomerId(customerId)
      .slice()
      .sort((a, b) =>
        `${a.firstName} ${a.lastName}`.localeCompare(
          `${b.firstName} ${b.lastName}`
        )
      );

    const resetFormState = () => {
      setIsDirty(false);
      setSubmitError("");
      (Object.keys(emptyErrors) as (keyof typeof emptyErrors)[]).forEach(
        (field) => clearError(field)
      );
    };

    const openForm = (contact?: IContact) => {
      setFormModel(
        contact ? { ...contact } : { ...emptyContact, customerId, customerName }
      );
      resetFormState();
    };

    const closeForm = () => {
      setFormModel(undefined);
      resetFormState();
    };

    const updateField = (field: keyof IContact, value: string) => {
      setFormModel((currentProps) =>
        currentProps ? { ...currentProps, [field]: value } : currentProps
      );
      clearError(field);
      setIsDirty(true);
    };

    const validateForm = (model: IContact) => {
      let isFormValid = true,
        err = "";

      err = validateTxt(model.firstName, [
        TextValidateTypeEnum.notEmpty,
        TextValidateTypeEnum.twoCharsMin,
        TextValidateTypeEnum.startWithTwoLetters,
      ]);
      isFormValid = updateFieldError("firstName", err) && isFormValid;

      err = validateTxt(model.lastName, [
        TextValidateTypeEnum.notEmpty,
        TextValidateTypeEnum.twoCharsMin,
        TextValidateTypeEnum.startWithTwoLetters,
      ]);
      isFormValid = updateFieldError("lastName", err) && isFormValid;

      err = validteEmail(model.email, [
        TextValidateTypeEnum.notEmpty,
        TextValidateTypeEnum.emailValid,
      ]);
      isFormValid = updateFieldError("email", err) && isFormValid;

      err = validtePhone(model.phone, [TextValidateTypeEnum.phoneValid]);
      isFormValid = updateFieldError("phone", err) && isFormValid;

      return isFormValid;
    };

    const handleSubmit = async () => {
      if (!formModel) return;
      setIsDirty(false);
      if (!validateForm(formModel)) return;

      const response =
        formModel.id === 0
          ? await customersContactsStore.addContact(formModel)
          : await customersContactsStore.updateContact(formModel);

      if (response.isSuccess) {
        await customersContactsStore.getContactsList();
        closeForm();
      } else {
        setSubmitError("An Error Occurred Please Try Again Later.");
      }
    };

    const deleteContact = async (contact: IContact) => {
      const isDelete = await generalStore.alertConfirmDialog(
        AlertConfirmDialogEnum.Confirm,
        "Delete Contact",
        "Are you sure you want to delete this contact?"
      );

      if (!isDelete) return;

      const response = await customersContactsStore.deleteContact(contact.id);

      if (response.isSuccess) {
        if (contact.id === customersContactsStore.selectedContact?.id) {
          generalStore.showContactFormDialog = false;
          customersContactsStore.selectedContact = undefined;
        }
        if (contact.id === formModel?.id) closeForm();
        await customersContactsStore.getContactsList();
      } else {
        setSubmitError("An Error Occurred Please Try Again Later.");
      }
    };

    return (
      <Box
        mt={2}
        p={{ xs: 1.5, sm: 2 }}
        sx={{ border: 1, borderColor: "divider", borderRadius: 2 }}
      >
        <Stack direction="row" alignItems="center" gap={1}>
          <Typography variant="subtitle1">Contacts</Typography>
          <Button
            size="small"
            onClick={() => openForm()}
            startIcon={<GoPlus />}
          >
            Add Contact
          </Button>
        </Stack>

        <List dense sx={{ width: "100%", bgcolor: "background.paper" }}>
          {contacts.length === 0 && (
            <ListItem>
              <ListItemText secondary="No contacts for this customer" />
            </ListItem>
          )}
          {contacts.map((contact) => (
            <ListItem
              key={contact.id}
              selected={contact.id === formModel?.id}
              sx={{ borderBottom: "1px solid #f1f1f1" }}
              secondaryAction={
                <Stack direction="row">
                  <IconButton
                    size="small"
                    aria-label="edit contact"
                    onClick={() => openForm(contact)}
                  >
                    <MdEdit />
                  </IconButton>
                  <IconButton
                    size="small"
                    aria-label="delete contact"
                    onClick={() => deleteContact(contact)}
                  >
                    <MdDelete />
                  </IconButton>
                </Stack>
              }
            >
              <ListItemText
                primary={`${contact.firstName} ${contact.lastName}${
                  contact.role ? ` - ${contact.role}` : ""
                }`}
                secondary={[contact.email, contact.phone]
                  .filter(Boolean)
                  .join(" | ")}
              />
            </ListItem>
          ))}
        </List>

        {formModel && (
          <Box
            mt={1}
            p={{ xs: 1.5, sm: 2 }}
            sx={{ border: 1, borderColor: "divider", borderRadius: 2 }}
          >
            <Typography variant="subtitle2">
              {formModel.id === 0 ? "Add Contact" : "Edit Contact"}
            </Typography>
            <form noValidate spellCheck="false">
              <Grid container columnSpacing={2}>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    required
                    autoFocus
                    margin="dense"
                    type="text"
                    id="contactFirstName"
                    label="First Name"
                    variant="outlined"
                    onChange={(e) => updateField("firstName", e.target.value)}
                    error={errModel.firstName !== ""}
                    helperText={errModel.firstName}
                    value={formModel.firstName || ""}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    required
                    margin="dense"
                    type="text"
                    id="contactLastName"
                    label="Last Name"
                    variant="outlined"
                    onChange={(e) => updateField("lastName", e.target.value)}
                    error={errModel.lastName !== ""}
                    helperText={errModel.lastName}
                    value={formModel.lastName || ""}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    margin="dense"
                    type="text"
                    id="contactRole"
                    label="Role"
                    variant="outlined"
                    onChange={(e) => updateField("role", e.target.value)}
                    value={formModel.role || ""}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    fullWidth
                    margin="dense"
                    type="text"
                    id="contactPhone"
                    label="Phone"
                    variant="outlined"
                    onChange={(e) => updateField("phone", e.target.value)}
                    error={errModel.phone !== ""}
                    helperText={errModel.phone}
                    value={formModel.phone || ""}
                  />
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    required
                    margin="dense"
                    type="text"
                    id="contactEmail"
                    label="Email"
                    variant="outlined"
                    onChange={(e) => updateField("email", e.target.value)}
                    error={errModel.email !== ""}
                    helperText={errModel.email}
                    value={formModel.email || ""}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Stack
                    direction="row"
                    justifyContent="flex-end"
                    gap={1}
                    mt={1}
                  >
                    <Button color="secondary" onClick={closeForm}>
                      Cancel
                    </Button>
                    <Button
                      disabled={!isDirty}
                      color="secondary"
                      onClick={handleSubmit}
                    >
                      Save Contact
                    </Button>
                  </Stack>
                </Grid>
              </Grid>
            </form>
          </Box>
        )}

        <FormHelperText error>{submitError}</FormHelperText>
      </Box>
    );
  }
);
