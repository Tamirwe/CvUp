import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { SetStateAction, useEffect, useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { UserRegistrationModel } from "../../models/Auth";
import {
  emailValidte,
  passwordValidate,
  textFieldValidte,
} from "../../utils/Validation";

interface textFieldInterface {
  error?: boolean;
  value: string;
  helperText?: string;
}

export const RegisterForm = () => {
  const rootStore = useStore();
  const { authStore } = rootStore;

  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));
  const [isDirty, setIsDirty] = useState(false);
  const [isTermsValid, setIsTermsValid] = useState(true);
  const [isShowPassword, setIsShowPassword] = useState(false);
  const [firstNameProps, setFirstNameProps] = useState<textFieldInterface>({
    value: "",
  });
  const [lastNameProps, setLastNameProps] = useState<textFieldInterface>({
    value: "",
  });
  const [companyProps, setCompanyProps] = useState<textFieldInterface>({
    value: "",
  });
  const [emailProps, setEmailProps] = useState<textFieldInterface>({
    value: "",
  });
  const [passwordProps, setPasswordProps] = useState<textFieldInterface>({
    value: "",
  });
  const [isTerms, setTerms] = useState(false);

  useEffect(() => {
    window.addEventListener("keydown", handleKeyDown);

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
    };
  }, []);

  const handleKeyDown = () => {
    setIsDirty(true);
  };

  const validateField = (
    typeValidate: string,
    field: textFieldInterface,
    setField: (value: SetStateAction<textFieldInterface>) => void
  ) => {
    let isFormValid = true;
    let fieldError = "";

    switch (typeValidate) {
      case "email":
        fieldError = emailValidte(field.value);
        break;
      case "password":
        fieldError = passwordValidate(field.value);
        break;
      default:
        fieldError = textFieldValidte(field.value, true, true, true);
        break;
    }

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
    let isFormValid = true;

    isFormValid = validateField("text", firstNameProps, setFirstNameProps);
    isFormValid =
      validateField("text", lastNameProps, setLastNameProps) && isFormValid;
    isFormValid =
      validateField("text", companyProps, setCompanyProps) && isFormValid;
    isFormValid =
      validateField("email", emailProps, setEmailProps) && isFormValid;
    isFormValid =
      validateField("password", passwordProps, setPasswordProps) && isFormValid;

    return isFormValid;
  };

  const submitForm = () => {
    const registrationInfo: UserRegistrationModel = {
      firstName: firstNameProps.value,
      lastName: lastNameProps.value,
      companyName: companyProps.value,
      email: emailProps.value,
      password: passwordProps.value,
    };
    authStore.registerUser(registrationInfo);
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item>
          <Grid container spacing={matchDownSM ? 0 : 2}>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                required
                margin="normal"
                type="text"
                id="firstNameTxt"
                label="First Name"
                variant="outlined"
                onChange={(e) => {
                  setFirstNameProps((currentProps) => ({
                    ...currentProps,
                    value: e.target.value,
                  }));
                }}
                error={firstNameProps.error}
                helperText={firstNameProps.helperText}
                value={firstNameProps.value}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                required
                margin="normal"
                type="text"
                id="lastNameTxt"
                label="Last Name"
                variant="outlined"
                onChange={(e) => {
                  setLastNameProps((currentProps) => ({
                    ...currentProps,
                    value: e.target.value,
                  }));
                }}
                error={lastNameProps.error}
                helperText={lastNameProps.helperText}
                value={lastNameProps.value}
              />
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            margin="normal"
            type="text"
            id="companyNameTxt"
            label="Company"
            variant="outlined"
            onChange={(e) => {
              setCompanyProps((currentProps) => ({
                ...currentProps,
                value: e.target.value,
              }));
            }}
            error={companyProps.error}
            helperText={companyProps.helperText}
            value={companyProps.value}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            margin="normal"
            type="text"
            id="userNameTxt"
            label="Email Address / Username"
            variant="outlined"
            onChange={(e) => {
              setEmailProps((currentProps) => ({
                ...currentProps,
                value: e.target.value,
              }));
            }}
            error={emailProps.error}
            helperText={emailProps.helperText}
            value={emailProps.value}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            margin="normal"
            type={isShowPassword ? "text" : "password"}
            id="passwordTxt"
            label="Password"
            variant="outlined"
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    aria-label="toggle password visibility"
                    onClick={() => setIsShowPassword(!isShowPassword)}
                    edge="end"
                  >
                    {isShowPassword ? (
                      <MdOutlineVisibilityOff />
                    ) : (
                      <MdOutlineVisibility />
                    )}
                  </IconButton>
                </InputAdornment>
              ),
            }}
            onChange={(e) => {
              setPasswordProps((currentProps) => ({
                ...currentProps,
                value: e.target.value,
              }));
            }}
            error={passwordProps.error}
            helperText={passwordProps.helperText}
            value={passwordProps.value}
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={isTerms}
                onChange={(e) => {
                  setTerms(e.target.checked);
                  setIsDirty(true);
                  setIsTermsValid(true);
                }}
              />
            }
            label={
              <Typography variant="subtitle1">
                Agree with &nbsp;
                <Typography variant="subtitle2" component={Link} to="/terms">
                  Terms & Condition.
                </Typography>
              </Typography>
            }
          />
        </Grid>
        {!isTermsValid && (
          <Grid item xs={12}>
            <FormHelperText error>
              you must agree to the terms and conditions
            </FormHelperText>
          </Grid>
        )}
        <Grid item xs={12}>
          <Box sx={{ mt: 2 }}>
            <Button
              disabled={!isDirty}
              fullWidth
              size="large"
              type="submit"
              variant="contained"
              color="secondary"
              onClick={() => {
                setIsDirty(false);
                if (validateForm()) {
                  if (isTerms) {
                    submitForm();
                  } else {
                    setIsTermsValid(false);
                  }
                }
              }}
            >
              Sign up
            </Button>
          </Box>
        </Grid>
      </Grid>
    </form>
  );
};
