import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";

import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { textFieldValidte } from "../../utils/Validation";

interface IProps {
  department?: IIdName;
  onClose: () => void;
}

export const DepartmentForm = ({ department, onClose }: IProps) => {
  const { generalStore } = useStore();

  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<IIdName>({
    id: 0,
    name: "",
  });
  const [formValError, setFormValError] = useState({
    name: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    name: "",
  });

  useEffect(() => {
    department && setFormModel({ ...department });
  }, [department]);

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

    let errTxt = textFieldValidte(formModel.name, true, true, true);
    isFormValid = updateFieldError("name", errTxt) && isFormValid;

    return isFormValid;
  };

  const submitForm = async () => {
    const response = await generalStore.addUpdateDepartment(formModel);

    if (response.isSuccess) {
      onClose();
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
              updateFieldError("name", "");
            }}
            error={formValError.name}
            helperText={formValErrorTxt.name}
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
                <Button
                  fullWidth
                  variant="contained"
                  color="secondary"
                  onClick={() => onClose()}
                >
                  Cancel
                </Button>
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
              </Stack>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};
