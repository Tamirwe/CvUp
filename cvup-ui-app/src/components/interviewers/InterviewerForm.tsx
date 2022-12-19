import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IInterviewer } from "../../models/AuthModels";
import { CrudTypes } from "../../models/GeneralEnums";
import { textFieldValidte } from "../../utils/Validation";

interface IProps {
  interviewer: IInterviewer;
  crudType?: CrudTypes;
  onSaved: () => void;
  onCancel: () => void;
}

export const InterviewerForm = ({
  interviewer,
  crudType,
  onSaved,
  onCancel,
}: IProps) => {
  const { authStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<IInterviewer>(interviewer);
  const [formValError, setFormValError] = useState({
    firstName: false,
    lastName: false,
    email: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    firstName: "",
    lastName: "",
    email: "",
  });

  useEffect(() => {
    interviewer && setFormModel({ ...interviewer });
  }, [interviewer]);

  const updateFieldError = (field: string, errTxt: string) => {
    const isValid = errTxt === "" ? true : false;
    setIsDirty(true);
    setSubmitError("");

    setFormValErrorTxt((currentProps) => ({
      ...currentProps,
      [field]: errTxt,
    }));
    setFormValError((currentProps) => ({
      ...currentProps,
      [field]: isValid === false,
    }));

    return isValid;
  };

  const validateForm = () => {
    let isFormValid = true;
    let errTxt = textFieldValidte(formModel.firstName, true, true, true);
    isFormValid = updateFieldError("firstName", errTxt) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await authStore.addUpdateInterviewer(formModel);

    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  const deleteRecord = async () => {
    const response = await authStore.deleteInterviewer(formModel);

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
            disabled={crudType === CrudTypes.Delete}
            margin="normal"
            type="text"
            id="fnameInp"
            label="First Name"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                firstName: e.target.value,
              }));
              updateFieldError("firstName", "");
            }}
            error={formValError.firstName}
            helperText={formValErrorTxt.firstName}
            value={formModel.firstName}
          />
        </Grid>
        <Grid item xs={12}>
          <FormHelperText error>{submitError}</FormHelperText>
        </Grid>
        <Grid item xs={12} mt={2}>
          <Grid container justifyContent="flex-end">
            <Grid item>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button
                  fullWidth
                  variant="contained"
                  color="secondary"
                  onClick={() => onCancel()}
                >
                  Cancel
                </Button>
                {crudType === CrudTypes.Delete ? (
                  <Button
                    fullWidth
                    variant="contained"
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
                    variant="contained"
                    color="secondary"
                    onClick={() => {
                      setIsDirty(false);
                      if (validateForm()) {
                        submitForm();
                      }
                    }}
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
