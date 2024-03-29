import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import {
  AlertConfirmDialogEnum,
  CrudTypesEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { textFieldValidte, validateTxt } from "../../utils/Validation";
import { ICustomer } from "../../models/GeneralModels";
import { isMobile } from "react-device-detect";
import { format } from "date-fns";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const CustomerForm = ({ onSaved, onCancel }: IProps) => {
  const { customersContactsStore, generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<ICustomer>({
    id: 0,
    name: "",
    descr: "",
    address: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
  });

  useEffect(() => {
    if (customersContactsStore.selectedCustomer) {
      setCrudType(CrudTypesEnum.Update);
      setFormModel({ ...customersContactsStore.selectedCustomer });
    }
  }, []);

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.name, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("name", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (validateForm()) {
      let response;

      if (crudType === CrudTypesEnum.Insert) {
        response = await customersContactsStore.addCustomer(formModel);
      } else {
        response = await customersContactsStore.updateCustomer(formModel);
      }

      if (response.isSuccess) {
        customersContactsStore.setCustomerAddedUpdated(response.data);
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deleteRecord = async () => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete Customer",
      "Are you sure you want to delete this customer?"
    );

    if (isDelete) {
      const response = await customersContactsStore.deleteCustomer(
        formModel.id
      );

      if (response.isSuccess) {
        onSaved();

        if (
          formModel.id === customersContactsStore.selectedContact?.customerId
        ) {
          generalStore.showContactFormDialog = false;
          customersContactsStore.getContactsList();
        }
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            autoFocus
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="name"
            label="Name"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                name: e.target.value,
              }));
              clearError("name");
              setIsDirty(true);
            }}
            error={errModel.name !== ""}
            helperText={errModel.name}
            value={formModel.name}
          />
        </Grid>

        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350, direction: "rtl" }}
            inputProps={{ maxLength: 255 }}
            fullWidth
            autoFocus
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="address"
            label="Address"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                address: e.target.value,
              }));
              setIsDirty(true);
            }}
            value={formModel.address}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{
              minWidth: 350,
              direction: "rtl",
            }}
            multiline
            rows={5}
            inputProps={{ maxLength: 255 }}
            fullWidth
            autoFocus
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="descr"
            label="Description"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                descr: e.target.value,
              }));
              setIsDirty(true);
            }}
            value={formModel.descr}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            autoFocus
            disabled={true}
            margin="normal"
            type="text"
            id="created"
            label="Created"
            variant="outlined"
            value={
              formModel.created &&
              format(new Date(formModel.created), "MMM d, yyyy")
            }
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
};
