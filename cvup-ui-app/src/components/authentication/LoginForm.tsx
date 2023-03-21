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
import { useEffect, useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link, useNavigate } from "react-router-dom";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IUserLogin } from "../../models/AuthModels";
import { TextValidateTypeEnum } from "../../models/GeneralEnums";
import { passwordValidate, validteEmail } from "../../utils/Validation";

interface IProps {
  loginType: string;
}

export const LoginForm = ({ loginType }: IProps) => {
  const navigate = useNavigate();
  const { authStore, candsStore, generalStore, positionsStore } = useStore();
  const params = new URLSearchParams(window.location.search);
  const [isDirty, setIsDirty] = useState(true);
  const [submitError, setSubmitError] = useState("");
  const [isShowPassword, setIsShowPassword] = useState(false);

  const [formModel, setFormModel] = useState<IUserLogin>({
    email: "",
    password: "",
    rememberMe: false,
    key: params.get("sk") || "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    email: "",
    password: "",
    rememberMe: false,
  });

  useEffect(() => {
    authStore.reset();
    candsStore.reset();
    generalStore.reset();
    positionsStore.reset();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validteEmail(formModel.email, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.emailValid,
    ]);
    isFormValid = updateFieldError("email", err) && isFormValid;

    err = passwordValidate(formModel.password);
    isFormValid = updateFieldError("password", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    let response;

    setIsDirty(false);
    setSubmitError("");

    if (validateForm()) {
      const isSuccess = await authStore.login(formModel, loginType);

      if (isSuccess) {
        navigate("/");
      } else {
        setSubmitError("Incorrect email address or password.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false" autoComplete="off">
      <Grid container>
        <Grid item xs={12}>
          <TextField
            fullWidth
            autoFocus
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
              setFormModel((currentProps) => ({
                ...currentProps,
                password: e.target.value,
              }));
              clearError("password");
              setIsDirty(true);
            }}
            InputLabelProps={{ shrink: true }}
            error={errModel.password !== ""}
            helperText={errModel.password}
            value={formModel.password}
          />
        </Grid>
        {loginType === "login" && (
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
                    checked={formModel.rememberMe}
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        rememberMe: e.target.checked,
                      }));
                      updateFieldError("rememberMe", "");
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
        )}
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
              onClick={handleSubmit}
            >
              Sign in
            </Button>
          </Box>
        </Grid>
      </Grid>
    </form>
  );
};
