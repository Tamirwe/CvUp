import {
  Box,
  Button,
  FormHelperText,
  Grid,
  TextField,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { useState } from "react";

import { useStore } from "../../Hooks/useStore";
import { PositionFormModel, textFieldInterface } from "../../models/AuthModels";
import { validateField } from "../../utils/Validation";

export const PositionForm = () => {
  const { authStore } = useStore();

  const theme = useTheme();
  const [submitError, setSubmitError] = useState("");

  const [nameProps, setNameProps] = useState<textFieldInterface>({
    value: "",
  });
  const [descrProps, setDescrProps] = useState<textFieldInterface>({
    value: "",
  });

  const validateForm = () => {
    let isFormValid = true;

    isFormValid = validateField("text", nameProps, setNameProps) && isFormValid;

    return isFormValid;
  };

  const submitForm = async () => {
    const formInfo: PositionFormModel = {
      name: nameProps.value,
      descr: descrProps.value,
    };

    // const response = await authStore.registerUser(registrationInfo);

    // if (response.isSuccess) {
    //   if (response.data === "duplicateUserPass") {
    //     return setSubmitError("Duplcate User Name and Password.");
    //   }
    // } else {
    //   return setSubmitError("An Error Occurred Please Try Again Later.");
    // }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            margin="normal"
            type="text"
            label="Title"
            variant="outlined"
            onChange={(e) => {
              setNameProps((currentProps) => ({
                ...currentProps,
                value: e.target.value,
              }));
            }}
            error={nameProps.error}
            helperText={nameProps.helperText}
            value={nameProps.value}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            margin="normal"
            type="text"
            multiline={true}
            rows={4}
            label="Description"
            variant="outlined"
            onChange={(e) => {
              setDescrProps((currentProps) => ({
                ...currentProps,
                value: e.target.value,
              }));
            }}
            error={descrProps.error}
            helperText={descrProps.helperText}
            value={descrProps.value}
          />
        </Grid>

        {submitError && (
          <Grid item xs={12}>
            <FormHelperText error>{submitError}</FormHelperText>
          </Grid>
        )}
        <Grid item xs={12}>
          <Box sx={{ mt: 2 }}>
            <Button
              fullWidth
              size="large"
              type="submit"
              variant="contained"
              color="secondary"
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();

                if (validateForm()) {
                  submitForm();
                }
              }}
            >
              Save
            </Button>
          </Box>
        </Grid>
      </Grid>
    </form>
  );
};
