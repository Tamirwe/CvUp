import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum, TextValidateTypeEnum } from "../../models/GeneralEnums";
import { textFieldValidte, validateTxt } from "../../utils/Validation";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const CustomerForm = ({ onSaved, onCancel }: IProps) => {
  const { customersContactsStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<IIdName>({
    id: 0,
    name: "",
  });
  const [formValError, setFormValError] = useState({
    name: false,
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
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deleteRecord = async () => {
    const response = await customersContactsStore.deleteCustomer(formModel.id);

    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="title"
            label="Team Name"
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
          <FormHelperText error>{submitError}</FormHelperText>
        </Grid>
        <Grid item xs={12} mt={2}>
          <Grid container justifyContent="flex-end">
            <Grid item>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button fullWidth color="secondary" onClick={() => onCancel()}>
                  Cancel
                </Button>
                {crudType === CrudTypesEnum.Delete ? (
                  <Button
                    fullWidth
                    color="warning"
                    onClick={() => {
                      deleteRecord();
                    }}
                  >
                    Delete
                  </Button>
                ) : (
                  <Button
                    disabled={!isDirty}
                    fullWidth
                    color="secondary"
                    onClick={handleSubmit}
                  >
                    Save
                  </Button>
                )}
              </Stack>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};
