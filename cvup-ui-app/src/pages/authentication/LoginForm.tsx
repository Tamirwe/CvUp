import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { SetStateAction, useEffect, useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { textFieldInterface, UserLoginModel } from "../../models/AuthModels";
import {
  emailValidte,
  passwordValidate,
  textFieldValidte,
} from "../../utils/Validation";

export const LoginForm = () => {
  const rootStore = useStore();
  const { authStore } = rootStore;
  const params = new URLSearchParams(window.location.search);

  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [isShowPassword, setIsShowPassword] = useState(false);
  const [emailProps, setEmailProps] = useState<textFieldInterface>({
    value: "",
  });
  const [passwordProps, setPasswordProps] = useState<textFieldInterface>({
    value: "",
  });

  const [isRemember, setIsRemember] = useState(false);

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
    setSubmitError("");

    let isFormValid = true;

    isFormValid =
      validateField("email", emailProps, setEmailProps) && isFormValid;
    isFormValid =
      validateField("password", passwordProps, setPasswordProps) && isFormValid;

    if (!isFormValid) {
      setSubmitError("Incorrect email address or password, please try again");
    }
    return isFormValid;
  };

  const submitForm = async () => {
    const loginInfo: UserLoginModel = {
      email: emailProps.value,
      password: passwordProps.value,
      rememberMe: isRemember,
      key: params.get("sk") || "",
    };

    const response = await authStore.loginUser(loginInfo);

    if (!response.isSuccess) {
      setSubmitError("Incorrect email address or password, please try again");
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
    <form noValidate spellCheck="false">
      <Grid container>
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
            value={passwordProps.value}
          />
        </Grid>
        <Grid item xs={12}>
          <Stack
            direction="row"
            alignItems="center"
            justifyContent="space-between"
            spacing={1}
          >
            <FormControlLabel
              control={
                <Checkbox
                  checked={isRemember}
                  onChange={(e) => {
                    setIsRemember(e.target.checked);
                  }}
                />
              }
              label={
                <Typography variant="subtitle2">Keep me sign in</Typography>
              }
            />
            <Typography
              component={Link}
              to="/forgot-password"
              variant="subtitle2"
            >
              Forgot Password?
            </Typography>
          </Stack>
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
              Sign in
            </Button>
          </Box>
        </Grid>
      </Grid>
    </form>
  );
};
