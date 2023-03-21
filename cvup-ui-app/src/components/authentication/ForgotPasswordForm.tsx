import {
  Box,
  Button,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { useState } from "react";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IForgotPassword } from "../../models/AuthModels";
import { TextValidateTypeEnum } from "../../models/GeneralEnums";
import { ISelectBox } from "../../models/GeneralModels";
import { emailValidte, validteEmail } from "../../utils/Validation";

interface props {
  resetPasswordSent: (email: string) => void;
}

export const ForgotPasswordForm = (props: props) => {
  const { authStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [userCompanies, setUserCompanies] = useState<ISelectBox[]>([]);

  const [formModel, setFormModel] = useState<IForgotPassword>({
    email: "",
    companyId: 0,
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    email: "",
    companyId: "",
  });

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validteEmail(formModel.email, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.emailValid,
    ]);
    isFormValid = updateFieldError("email", err) && isFormValid;

    if (userCompanies?.length > 0 && formModel.companyId === 0) {
      isFormValid =
        updateFieldError("companyId", "please select a company") && isFormValid;
    }

    return isFormValid;
  };

  const handleSubmit = async () => {
    let response;

    setIsDirty(false);
    setSubmitError("");

    if (validateForm()) {
      const response = await authStore.forgotPassword(formModel);

      if (response.isSuccess) {
        if (response.data === "emailSent") {
          props.resetPasswordSent(formModel.email);
        } else if (response.data === "userNotFound") {
          setSubmitError("Email not found");
        } else if (Array.isArray(response.data)) {
          setUserCompanies(response.data);
          updateFieldError("companyId", "please select a company");
        }
      } else {
        setSubmitError("Incorrect email address, please try again later");
      }
    }
  };

  return (
    <form noValidate>
      <Grid item xs={12}>
        <TextField
          fullWidth
          required
          InputLabelProps={{ shrink: true }}
          margin="normal"
          type="text"
          id="emailTxt"
          label="Email Address / Username"
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
          value={formModel.email}
        />
      </Grid>
      {userCompanies.length > 0 && (
        <Grid item xs={12} mt={2}>
          <FormControl fullWidth>
            <InputLabel id="companylabel">Company</InputLabel>
            <Select
              labelId="companylabel"
              id="companySelect"
              label="Company"
              onChange={(e) => {
                setFormModel((currentProps) => ({
                  ...currentProps,
                  companyId: parseInt(e.target.value),
                }));
                updateFieldError("companyId", "");
              }}
              error={errModel.companyId !== ""}
              value={formModel.companyId.toString()}
            >
              <MenuItem value="0" key="0"></MenuItem>
              {userCompanies?.map((company) => {
                return (
                  <MenuItem key={company.id} value={company.id}>
                    {company.name}
                  </MenuItem>
                );
              })}
            </Select>
            <FormHelperText error>{errModel.companyId}</FormHelperText>
          </FormControl>
        </Grid>
      )}
      {submitError && (
        <Grid item xs={12}>
          <FormHelperText error>{submitError}</FormHelperText>
        </Grid>
      )}
      <Grid item xs={12}>
        <Box sx={{ mt: 4 }}>
          <Button
            fullWidth
            disabled={!isDirty}
            size="large"
            type="submit"
            variant="contained"
            color="secondary"
            onClick={handleSubmit}
          >
            Request Password Reset
          </Button>
        </Box>
      </Grid>
    </form>
  );
};
