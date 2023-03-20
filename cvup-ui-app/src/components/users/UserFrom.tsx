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
import { useEffect, useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IUser } from "../../models/AuthModels";
import { CrudTypesEnum, TextValidateTypeEnum } from "../../models/GeneralEnums";
import {
  passwordValidate,
  validateTxt,
  validteEmail,
  validtePhone,
} from "../../utils/Validation";

interface IProps {
  registerFormComplete: (email: string) => void;
}

export const UserFrom = (props: IProps) => {
  const { authStore } = useStore();
  const theme = useTheme();
  const matchDownMd = useMediaQuery(theme.breakpoints.down("md"));

  const [isShowPassword, setIsShowPassword] = useState(false);
  const [isTerms, setIsTerms] = useState(false);
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<IUser>({
    id: 0,
    firstName: "",
    lastName: "",
    companyName: "",
    companyId: 0,
    email: "",
    phone: "",
    password: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    firstName: "",
    lastName: "",
    companyName: "",
    email: "",
    phone: "",
    password: "",
  });

  useEffect(() => {
    if (authStore.selectedUser) {
      setCrudType(CrudTypesEnum.Update);
      setFormModel({ ...authStore.selectedUser });
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.firstName, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("firstName", err) && isFormValid;

    err = validateTxt(formModel.lastName, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("lastName", err) && isFormValid;

    err = validateTxt(formModel.companyName, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
    ]);
    isFormValid = updateFieldError("companyName", err) && isFormValid;

    err = validteEmail(formModel.email, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.emailValid,
    ]);
    isFormValid = updateFieldError("email", err) && isFormValid;

    err = validtePhone(formModel.phone, [TextValidateTypeEnum.phoneValid]);
    isFormValid = updateFieldError("phone", err) && isFormValid;

    err = passwordValidate(formModel.password);
    isFormValid = updateFieldError("password", err) && isFormValid;

    if (!isTerms) {
      setSubmitError("you must agree to the terms and conditions");
      isFormValid = false;
    }

    return isFormValid;
  };

  const handleSubmit = async () => {
    let response;

    setIsDirty(false);

    if (validateForm()) {
      if (crudType === CrudTypesEnum.Insert) {
        response = await authStore.registerUser(formModel);
      } else {
        // response = await authStore.updateUser(formModel);
      }

      if (response && response.isSuccess) {
        if (response.data === "duplicateUserPass") {
          return setSubmitError("Duplcate User Name and Password.");
        }

        props.registerFormComplete(formModel.email);
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item>
          <Grid container spacing={matchDownMd ? 0 : 2}>
            <Grid item xs={6}>
              <TextField
                fullWidth
                margin="normal"
                type="text"
                id="fnameInp"
                label="First Name"
                variant="outlined"
                onChange={(e) => {
                  setFormModel((currentProps) => ({
                    ...currentProps,
                    firstName: e.target.value,
                  }));
                  clearError("firstName");
                  setIsDirty(true);
                }}
                error={errModel.firstName !== ""}
                helperText={errModel.firstName}
                value={formModel.firstName}
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                fullWidth
                margin="normal"
                type="text"
                id="lastNameInp"
                label="Last Name"
                variant="outlined"
                onChange={(e) => {
                  setFormModel((currentProps) => ({
                    ...currentProps,
                    lastName: e.target.value,
                  }));
                  clearError("lastName");
                  setIsDirty(true);
                }}
                error={errModel.lastName !== ""}
                helperText={errModel.lastName}
                value={formModel.lastName}
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
              setFormModel((currentProps) => ({
                ...currentProps,
                companyName: e.target.value,
              }));
              updateFieldError("companyName", "");
            }}
            error={errModel.companyName !== ""}
            helperText={errModel.companyName}
            value={formModel.companyName}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            InputLabelProps={{ shrink: true }}
            margin="normal"
            type="text"
            id="userNameTxt"
            label="Email Address / Username"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                email: e.target.value,
              }));
              updateFieldError("email", "");
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
              updateFieldError("password", "");
            }}
            error={errModel.password !== ""}
            helperText={errModel.password}
            value={formModel.password}
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={isTerms}
                onChange={(e) => {
                  setIsTerms(e.target.checked);
                  setIsDirty(true);
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
        {submitError && (
          <Grid item xs={12}>
            <FormHelperText error>{submitError}</FormHelperText>
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
              onClick={handleSubmit}
            >
              Sign up
            </Button>
          </Box>
        </Grid>
      </Grid>
    </form>
  );
};
