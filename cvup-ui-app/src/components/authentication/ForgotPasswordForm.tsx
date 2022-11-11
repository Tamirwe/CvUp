import {
  Box,
  Button,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import { SetStateAction, useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { textFieldInterface } from "../../models/AuthModels";
import { SelectModel } from "../../models/GeneralModels";
import { emailValidte } from "../../utils/Validation";

interface props {
  resetPasswordSent: (email: string) => void;
}

export const ForgotPasswordForm = (props: props) => {
  const { authStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [emailProps, setEmailProps] = useState<textFieldInterface>({
    value: "",
  });

  const [companyId, setCompanyId] = useState(0);
  const [userCompanies, setUserCompanies] = useState<SelectModel[]>();

  const validateField = (
    field: textFieldInterface,
    setField: (value: SetStateAction<textFieldInterface>) => void
  ) => {
    let isFormValid = true;
    let fieldError = emailValidte(field.value);

    setField((currentProps) => ({
      ...currentProps,
      error: false,
      helperText: "",
    }));

    if (fieldError) {
      isFormValid = false;
      setField((currentProps) => ({
        ...currentProps,
        error: true,
        helperText: fieldError,
      }));
    }

    return isFormValid;
  };

  const validateForm = () => {
    setSubmitError("");

    let isFormValid = true;

    isFormValid = validateField(emailProps, setEmailProps) && isFormValid;

    if (!isFormValid) {
      setSubmitError("Incorrect email address, please try again");
    }
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await authStore.forgotPassword({
      email: emailProps.value,
      companyId,
    });

    if (!response.isSuccess) {
      setSubmitError("Incorrect email address or password, please try again");
    } else if (Array.isArray(response.data)) {
      setUserCompanies(response.data);
    } else if (response.data.data === "userNotFound") {
      setSubmitError("Email not found");
    } else if (response.data.data === "emailSent") {
      props.resetPasswordSent(emailProps.value);
    }
  };

  useEffect(() => {
    window.addEventListener("keydown", handleKeyDown);

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
    };
  }, []);

  const handleKeyDown = () => {
    setIsDirty(true);
    setSubmitError("");
  };

  return (
    <form noValidate>
      <Grid item xs={12}>
        <TextField
          fullWidth
          required
          margin="normal"
          type="text"
          id="emailTxt"
          label="Email Address / Username"
          variant="outlined"
          onChange={(e) => {
            setEmailProps((currentProps) => ({
              ...currentProps,
              value: e.target.value,
            }));
          }}
          value={emailProps.value}
        />
      </Grid>
      {userCompanies?.length && (
        <Grid item xs={12} mt={2}>
          <FormControl fullWidth>
            <InputLabel id="companylabel">Company</InputLabel>
            <Select
              labelId="companylabel"
              id="companySelect"
              value={companyId.toString()}
              label="Company"
              onChange={(event: SelectChangeEvent) => {
                setIsDirty(true);
                setCompanyId(parseInt(event.target.value));
              }}
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
            <FormHelperText error>
              {!companyId && "please select a company"}
            </FormHelperText>
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
            onClick={(e) => {
              e.stopPropagation();
              e.preventDefault();

              setIsDirty(false);

              if (validateForm()) {
                submitForm();
              }
            }}
          >
            Request Password Reset
          </Button>
        </Box>
      </Grid>
    </form>
  );
};
