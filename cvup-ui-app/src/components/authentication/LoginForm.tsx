import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  MenuItem,
  Select,
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
import {
  IApiUrl,
  IAppSettings,
  IAppSettingsFile,
} from "../../models/GeneralModels";

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
  const [serversApi, setServersApi] = useState<IApiUrl[]>();
  const [apiUrl, setApiUrl] = useState("");
  const [apiUrlsSelectBoxShow, setApiUrlsSelectBoxShow] = useState(false);

  const [formModel, setFormModel] = useState<IUserLogin>({
    email: "",
    password: "",
    rememberMe: true,
    key: params.get("sk") || "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    email: "",
    password: "",
  });

  useEffect(() => {
    authStore.reset();
    candsStore.reset();
    generalStore.reset();
    positionsStore.reset();

    const apiUrlFlag = localStorage.getItem("apiUrlOn");

    // if (apiUrlFlag === "on") {
    // setApiUrlsSelectBoxShow(apiUrlFlag === "on");
    const storedSettings = localStorage.getItem("settings");
    const settingsObj: IAppSettings = JSON.parse(storedSettings || "");
    setApiUrl(settingsObj.apiUrl);

    fetch(`${process.env.PUBLIC_URL}/appSettings.json`)
      .then((res) => res.json())
      .then((data: IAppSettingsFile) => {
        setServersApi(data.servers);
        Object.freeze(serversApi);
      });
    // }

    // if (
    //   window.location.href.indexOf("10.100") === -1 &&
    //   window.location.href.indexOf("localhost") === -1
    // ) {
    //   setLocalStorageIP("http://" + window.location.hostname + ":8079/api/");
    // }
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

  const handleShowServersList = () => {
    setApiUrlsSelectBoxShow(!apiUrlsSelectBoxShow);
  };

  const setLocalStorageIP = (ip: string) => {
    const settingsObj: IAppSettings = JSON.parse(
      localStorage.getItem("settings") || ""
    );
    settingsObj.apiUrl = ip;
    localStorage.setItem("settings", JSON.stringify(settingsObj));
  };

  return (
    <form noValidate spellCheck="false" autoComplete="off">
      <Grid container>
        {apiUrlsSelectBoxShow && (
          <Grid item xs={12}>
            <Select
              labelId="companylabel"
              id="companySelect"
              label="Company"
              sx={{ width: "100%" }}
              onChange={(e) => {
                setApiUrl(e.target.value);
                setLocalStorageIP(e.target.value);
                window.location.reload();
              }}
              value={apiUrl}
            >
              {serversApi?.map((server, i) => {
                return (
                  <MenuItem key={i} value={server.apiUrl}>
                    {server.apiUrl}
                  </MenuItem>
                );
              })}
            </Select>
          </Grid>
        )}
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
        <Grid
          item
          xs={12}
          sx={{
            mt: 3,
            textAlign: "right",
            color: "#9e9e9e",
            cursor: "pointer",
            fontSize: "0.75rem",
          }}
          onClick={handleShowServersList}
        >
          {apiUrl || "Select Server"}
        </Grid>
      </Grid>
    </form>
  );
};
