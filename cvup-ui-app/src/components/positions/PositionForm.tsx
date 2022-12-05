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
import { PositionFormModel } from "../../models/AuthModels";

export const PositionForm = () => {
  const { authStore } = useStore();

  const theme = useTheme();
  const [submitError, setSubmitError] = useState("");

  const validateForm = () => {
    let isFormValid = true;

    return isFormValid;
  };

  const submitForm = async () => {};

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12}></Grid>
        <Grid item xs={12}></Grid>

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
