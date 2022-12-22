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
import { useStore } from "../../Hooks/useStore";
import { IUserLogin } from "../../models/AuthModels";
import { emailValidte, passwordValidate } from "../../utils/Validation";

interface IProps {
  loginType: string;
}

export const LoginForm = ({ loginType }: IProps) => {
  const navigate = useNavigate();
  const { authStore } = useStore();
  const params = new URLSearchParams(window.location.search);
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [isShowPassword, setIsShowPassword] = useState(false);

  const [formModel, setFormModel] = useState<IUserLogin>({
    email: "",
    password: "",
    rememberMe: false,
    key: params.get("sk") || "",
  });
  const [formValError, setFormValError] = useState({
    email: false,
    password: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    email: "",
    password: "",
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
    setSubmitError("");

    let isFormValid = true;
    let errTxt = emailValidte(formModel.email);
    isFormValid = updateFieldError("email", errTxt) && isFormValid;

    errTxt = passwordValidate(formModel.password);
    isFormValid = updateFieldError("password", errTxt) && isFormValid;

    return isFormValid;
  };

  const submitForm = async () => {
    const isSuccess = await authStore.login(formModel, loginType);

    if (isSuccess) {
      navigate("/");
    } else {
      setSubmitError("Incorrect email address or password.");
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
              updateFieldError("email", "");
            }}
            InputLabelProps={{ shrink: true }}
            value={formModel.email}
            error={formValError.email}
            helperText={formValErrorTxt.email}
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
              updateFieldError("password", "");
            }}
            InputLabelProps={{ shrink: true }}
            value={formModel.password}
            error={formValError.password}
            helperText={formValErrorTxt.password}
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
