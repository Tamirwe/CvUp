import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  Link,
  Stack,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { useState } from "react";
import { MdOutlineVisibilityOff, MdOutlineVisibility } from "react-icons/md";

import { useStore } from "../../Hooks/useStore";
import {
  IPosition,
  IUserRegistration,
  PositionFormModel,
} from "../../models/AuthModels";
import {
  textFieldValidte,
  emailValidte,
  passwordValidate,
} from "../../utils/Validation";

export const PositionForm = () => {
  const { positionsStore } = useStore();

  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));
  const [isDirty, setIsDirty] = useState(false);
  const [isShowPassword, setIsShowPassword] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [isTerms, setIsTerms] = useState(false);
  const [formModel, setFormModel] = useState<IPosition>({
    name: "",
    descr: "",
  });
  const [formValError, setFormValError] = useState({
    name: false,
    descr: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    name: "",
    descr: "",
  });

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
    const response = await positionsStore.addUpdatePosition(formModel);

    if (response.isSuccess) {
      if (response.data === "duplicateUserPass") {
        return setSubmitError("Duplcate User Name and Password.");
      }
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12} lg={6}>
          <Grid container>
            <Grid item xs={12}>
              <TextField
                fullWidth
                required
                margin="normal"
                type="text"
                id="title"
                label="Position title"
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
              <TextField
                fullWidth
                multiline
                rows={8}
                margin="normal"
                type="text"
                id="description"
                label="Description"
                variant="outlined"
                onChange={(e) => {
                  setFormModel((currentProps) => ({
                    ...currentProps,
                    descr: e.target.value,
                  }));
                  updateFieldError("descr", "");
                }}
                error={formValError.descr}
                helperText={formValErrorTxt.descr}
                value={formModel.descr}
              />
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={12} lg={6}></Grid>
        {submitError && (
          <Grid item xs={12}>
            <FormHelperText error>{submitError}</FormHelperText>
          </Grid>
        )}
        <Grid item xs={12} sx={{ mt: 2 }}>
          <Grid container>
            <Grid item xs={0} lg={10}></Grid>
            <Grid item xs={12} lg={2}>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button
                  disabled={!isDirty}
                  fullWidth
                  size="large"
                  variant="contained"
                  color="secondary"
                  onClick={(e) => {
                    e.stopPropagation();
                    e.preventDefault();

                    setIsDirty(false);
                    if (validateForm()) {
                      submitForm();
                    }
                  }}
                >
                  Cancel
                </Button>
                <Button
                  disabled={!isDirty}
                  fullWidth
                  size="large"
                  variant="contained"
                  color="secondary"
                  onClick={(e) => {
                    e.stopPropagation();
                    e.preventDefault();

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
