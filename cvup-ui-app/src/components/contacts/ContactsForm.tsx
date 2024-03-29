import {
  Autocomplete,
  Button,
  FormHelperText,
  Grid,
  IconButton,
  Stack,
  TextField,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdAdd } from "react-icons/md";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  CrudTypesEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { IContact } from "../../models/GeneralModels";
import {
  validateSelect,
  validateTxt,
  validteEmail,
  validtePhone,
} from "../../utils/Validation";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const ContactsForm = observer(({ onSaved, onCancel }: IProps) => {
  const { customersContactsStore, generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<IContact>({
    id: 0,
    customerId: 0,
    customerName: "",
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    role: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    customerId: "",
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    role: "",
  });

  useEffect(() => {
    customersContactsStore.setCustomerAddedUpdated(undefined);

    if (customersContactsStore.selectedContact) {
      setCrudType(CrudTypesEnum.Update);
      setFormModel({ ...customersContactsStore.selectedContact });
    }

    // (async () => {
    //   await Promise.all([customersContactsStore.getCustomersList()]);
    // })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (customersContactsStore.customerAddedUpdated) {
      const customer = customersContactsStore.customersList.find(
        (x) => x.id === customersContactsStore.customerAddedUpdated?.id
      );

      if (customer) {
        setFormModel((currentProps) => ({
          ...currentProps,
          customerId: customer.id,
          customerName: customer.name,
        }));
        clearError("customerId");
        setIsDirty(true);
      }
    }
  }, [customersContactsStore.customersList]);

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateSelect(formModel.customerId, [
      TextValidateTypeEnum.notSelected,
    ]);
    isFormValid = updateFieldError("customerId", err) && isFormValid;

    err = validateTxt(formModel.firstName, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("firstName", err) && isFormValid;

    err = validateTxt(formModel.lastName, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("lastName", err) && isFormValid;

    err = validteEmail(formModel.email, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.emailValid,
    ]);
    isFormValid = updateFieldError("email", err) && isFormValid;

    err = validtePhone(formModel.phone, [TextValidateTypeEnum.phoneValid]);
    isFormValid = updateFieldError("phone", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (validateForm()) {
      let response;

      if (crudType === CrudTypesEnum.Insert) {
        response = await customersContactsStore.addContact(formModel);
      } else {
        response = await customersContactsStore.updateContact(formModel);
      }

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deleteRecord = async () => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete Contact",
      "Are you sure you want to delete this contact?"
    );

    if (isDelete) {
      const response = await customersContactsStore.deleteContact(formModel.id);

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid item xs={12} lg={12} pt={2}>
        <Stack direction="row">
          <Autocomplete
            value={
              formModel.customerId > 0
                ? {
                    id: formModel.customerId,
                    label: formModel.customerName,
                  }
                : null
            }
            inputValue={formModel.customerName || ""}
            onInputChange={(event, newInputValue) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                customerName: newInputValue,
              }));
            }}
            disabled={crudType === CrudTypesEnum.Delete}
            onChange={(event, newValue) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                customerId: newValue?.id || 0,
                customerName: newValue?.label || "",
              }));
              clearError("customerId");
              setIsDirty(true);
            }}
            id="dsfs"
            isOptionEqualToValue={(option, value) => option.id === value.id}
            options={customersContactsStore.customersList.map((x) => ({
              id: x.id,
              label: x.name,
            }))}
            sx={{ width: "88%" }}
            renderInput={(params) => (
              <TextField
                autoFocus
                helperText={errModel.customerId}
                error={errModel.customerId !== ""}
                {...params}
                label="Customer"
              />
            )}
          />
          <IconButton
            size="medium"
            aria-label="toggle password visibility"
            onClick={() => (generalStore.showCustomersListDialog = true)}
            edge="end"
            sx={{
              marginTop: 1,
              marginLeft: 1,
              alignItems: "center",
              height: "40px",
              width: "40px",
            }}
          >
            <MdAdd />
          </IconButton>
        </Stack>
      </Grid>
      <Grid container>
        <Grid item xs={6}>
          <TextField
            fullWidth
            required
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="firstName"
            label="First Name"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                firstName: e.target.value,
              }));
              clearError("firstName");
              setIsDirty(true);
            }}
            error={errModel.firstName !== ""}
            helperText={errModel.firstName}
            value={formModel.firstName || ""}
          />
        </Grid>
        <Grid item xs={6}>
          <TextField
            fullWidth
            required
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="lastName"
            label="Last Name"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                lastName: e.target.value,
              }));
              clearError("lastName");
              setIsDirty(true);
            }}
            error={errModel.lastName !== ""}
            helperText={errModel.lastName}
            value={formModel.lastName || ""}
          />
        </Grid>

        <Grid item xs={12} lg={6}>
          <TextField
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="role"
            label="Role"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                role: e.target.value,
              }));
              clearError("role");
              setIsDirty(true);
            }}
            error={errModel.role !== ""}
            helperText={errModel.role}
            value={formModel.role || ""}
          />
        </Grid>
        <Grid item xs={12} lg={6}>
          <TextField
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="phone"
            label="Phone"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                phone: e.target.value,
              }));
              clearError("phone");
              setIsDirty(true);
            }}
            error={errModel.phone !== ""}
            helperText={errModel.phone}
            value={formModel.phone || ""}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            required
            InputLabelProps={{ shrink: true }}
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="email"
            label="Email"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                email: e.target.value,
              }));
              clearError("email");
              setIsDirty(true);
            }}
            error={errModel.email !== ""}
            helperText={errModel.email}
            value={formModel.email || ""}
          />
        </Grid>
        <Grid item xs={12}>
          <FormHelperText error>{submitError}</FormHelperText>
        </Grid>
        <Grid item xs={12} mt={2}>
          <Grid container justifyContent="space-between">
            <Grid item>
              {crudType === CrudTypesEnum.Update && (
                <Button
                  fullWidth
                  color="warning"
                  onClick={() => {
                    deleteRecord();
                  }}
                >
                  Delete
                </Button>
              )}
            </Grid>
            <Grid item>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button fullWidth color="secondary" onClick={() => onCancel()}>
                  Cancel
                </Button>

                <Button
                  disabled={!isDirty}
                  fullWidth
                  color="secondary"
                  onClick={handleSubmit}
                >
                  Save
                </Button>
              </Stack>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
});
