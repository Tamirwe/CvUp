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
  TextFieldProps,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { useEffect, useRef, useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import { IUser, IUserRegistration } from "../../models/AuthModels";
import { CrudTypesEnum, TextValidateTypeEnum } from "../../models/GeneralEnums";
import {
  passwordValidate,
  validateTxt,
  validteEmail,
} from "../../utils/Validation";

interface IProps {
  registerFormComplete: (email: string) => void;
}

export const RegisterForm = (props: IProps) => {
  const { authStore } = useStore();
  const theme = useTheme();
  const matchDownMd = useMediaQuery(theme.breakpoints.down("md"));
  
  const [isShowPassword, setIsShowPassword] = useState(false);
  const [isTerms, setIsTerms] = useState(false);
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<IUserRegistration>({
    firstName: "",
    lastName: "",
    companyName: "",
    email: "",
    password: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    firstName: "",
    lastName: "",
    companyName: "",
    email: "",
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
    setSubmitError("");
    
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
           
      <Grid container >
        <Grid item>
          <Grid container spacing={matchDownMd ? 0 : 2}>
            <Grid item xs={6}>
              <TextField
                // inputRef={input => input && input.focus()}
                fullWidth
                required
                InputLabelProps={{ shrink: true }}
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
                required
                InputLabelProps={{ shrink: true }}
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
            InputLabelProps={{ shrink: true }}
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
              clearError("companyName");
              setIsDirty(true);
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
            id="emailInp"
            label="Email Address / Username"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                email: e.target.value,
              }));
              clearError("lastName");
              setIsDirty(true);
            }}
            error={errModel.email !== ""}
            helperText={errModel.email}
            value={formModel.email }
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            required
            InputLabelProps={{ shrink: true }}
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
              clearError("lastName");
              setIsDirty(true);
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


// import {
//   Box,
//   Button,
//   Checkbox,
//   FormControlLabel,
//   FormHelperText,
//   Grid,
//   IconButton,
//   InputAdornment,
//   TextField,
//   Typography,
//   useMediaQuery,
//   useTheme,
// } from "@mui/material";
// import { useState } from "react";

// import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
// import { Link } from "react-router-dom";
// import { useStore } from "../../Hooks/useStore";
// import { IUserRegistration } from "../../models/AuthModels";
// import {
//   emailValidte,
//   passwordValidate,
//   textFieldValidte,
// } from "../../utils/Validation";

// interface IProps {
//   registerFormComplete: (email: string) => void;
// }

// export const RegisterForm = (props: IProps) => {
//   const { authStore } = useStore();
//   const theme = useTheme();
//   const matchDownMd = useMediaQuery(theme.breakpoints.down("md"));
//   const [isDirty, setIsDirty] = useState(false);
//   const [isShowPassword, setIsShowPassword] = useState(false);
//   const [submitError, setSubmitError] = useState("");
//   const [isTerms, setIsTerms] = useState(false);
//   const [formModel, setFormModel] = useState<IUserRegistration>({
//     firstName: "",
//     lastName: "",
//     companyName: "",
//     email: "",
//     password: "",
//   });
//   const [formValError, setFormValError] = useState({
//     firstName: false,
//     lastName: false,
//     companyName: false,
//     email: false,
//     password: false,
//   });
//   const [formValErrorTxt, setFormValErrorTxt] = useState({
//     firstName: "",
//     lastName: "",
//     companyName: "",
//     email: "",
//     password: "",
//   });

//   const updateFieldError = (field: string, errTxt: string) => {
//     const isValid = errTxt === "" ? true : false;
//     setIsDirty(true);
//     setSubmitError("");

//     setFormValErrorTxt((currentProps) => ({
//       ...currentProps,
//       [field]: errTxt,
//     }));
//     setFormValError((currentProps) => ({
//       ...currentProps,
//       [field]: isValid === false,
//     }));

//     return isValid;
//   };

//   const validateForm = () => {
//     let isFormValid = true;

//     let errTxt = textFieldValidte(formModel.firstName, true, true, true);
//     isFormValid = updateFieldError("firstName", errTxt) && isFormValid;

//     errTxt = textFieldValidte(formModel.lastName, true, true, true);
//     isFormValid = updateFieldError("lastName", errTxt) && isFormValid;

//     errTxt = textFieldValidte(formModel.companyName, true, true, true);
//     isFormValid = updateFieldError("companyName", errTxt) && isFormValid;

//     errTxt = emailValidte(formModel.email);
//     isFormValid = updateFieldError("email", errTxt) && isFormValid;

//     errTxt = passwordValidate(formModel.password);
//     isFormValid = updateFieldError("password", errTxt) && isFormValid;

//     if (!isTerms) {
//       setSubmitError("you must agree to the terms and conditions");
//       isFormValid = false;
//     }
//     return isFormValid;
//   };

//   const submitForm = async () => {
//     const response = await authStore.registerUser(formModel);

//     if (response.isSuccess) {
//       if (response.data === "duplicateUserPass") {
//         return setSubmitError("Duplcate User Name and Password.");
//       }

//       props.registerFormComplete(formModel.email);
//     } else {
//       return setSubmitError("An Error Occurred Please Try Again Later.");
//     }
//   };

//   return (
//     <form noValidate spellCheck="false">
//       <Grid container>
//         <Grid item>
//           <Grid container spacing={matchDownMd ? 0 : 2}>
//             <Grid item xs={12} sm={6}>
//               <TextField
//                 fullWidth
//                 required
//                 margin="normal"
//                 type="text"
//                 id="firstNameTxt"
//                 label="First Name"
//                 variant="outlined"
//                 onChange={(e) => {
//                   setFormModel((currentProps) => ({
//                     ...currentProps,
//                     firstName: e.target.value,
//                   }));
//                   updateFieldError("firstName", "");
//                 }}
//                 error={formValError.firstName}
//                 helperText={formValErrorTxt.firstName}
//                 value={formModel.firstName}
//               />
//             </Grid>
//             <Grid item xs={12} sm={6}>
//               <TextField
//                 fullWidth
//                 required
//                 margin="normal"
//                 type="text"
//                 id="lastNameTxt"
//                 label="Last Name"
//                 variant="outlined"
//                 onChange={(e) => {
//                   setFormModel((currentProps) => ({
//                     ...currentProps,
//                     lastName: e.target.value,
//                   }));
//                   updateFieldError("lastName", "");
//                 }}
//                 error={formValError.lastName}
//                 helperText={formValErrorTxt.lastName}
//                 value={formModel.firstName}
//               />
//             </Grid>
//           </Grid>
//         </Grid>
//         <Grid item xs={12}>
//           <TextField
//             fullWidth
//             required
//             margin="normal"
//             type="text"
//             id="companyNameTxt"
//             label="Company"
//             variant="outlined"
//             onChange={(e) => {
//               setFormModel((currentProps) => ({
//                 ...currentProps,
//                 companyName: e.target.value,
//               }));
//               updateFieldError("companyName", "");
//             }}
//             error={formValError.companyName}
//             helperText={formValErrorTxt.companyName}
//             value={formModel.companyName}
//           />
//         </Grid>
//         <Grid item xs={12}>
//           <TextField
//             fullWidth
//             required
//             margin="normal"
//             type="text"
//             id="userNameTxt"
//             label="Email Address / Username"
//             variant="outlined"
//             onChange={(e) => {
//               setFormModel((currentProps) => ({
//                 ...currentProps,
//                 email: e.target.value,
//               }));
//               updateFieldError("email", "");
//             }}
//             error={formValError.email}
//             helperText={formValErrorTxt.email}
//             value={formModel.email}
//           />
//         </Grid>
//         <Grid item xs={12}>
//           <TextField
//             fullWidth
//             required
//             margin="normal"
//             type={isShowPassword ? "text" : "password"}
//             id="passwordTxt"
//             label="Password"
//             variant="outlined"
//             InputProps={{
//               endAdornment: (
//                 <InputAdornment position="end">
//                   <IconButton
//                     aria-label="toggle password visibility"
//                     onClick={() => setIsShowPassword(!isShowPassword)}
//                     edge="end"
//                   >
//                     {isShowPassword ? (
//                       <MdOutlineVisibilityOff />
//                     ) : (
//                       <MdOutlineVisibility />
//                     )}
//                   </IconButton>
//                 </InputAdornment>
//               ),
//             }}
//             onChange={(e) => {
//               setFormModel((currentProps) => ({
//                 ...currentProps,
//                 password: e.target.value,
//               }));
//               updateFieldError("password", "");
//             }}
//             error={formValError.password}
//             helperText={formValErrorTxt.password}
//             value={formModel.password}
//           />
//           <FormControlLabel
//             control={
//               <Checkbox
//                 checked={isTerms}
//                 onChange={(e) => {
//                   setIsTerms(e.target.checked);
//                   setIsDirty(true);
//                 }}
//               />
//             }
//             label={
//               <Typography variant="subtitle1">
//                 Agree with &nbsp;
//                 <Typography variant="subtitle2" component={Link} to="/terms">
//                   Terms & Condition.
//                 </Typography>
//               </Typography>
//             }
//           />
//         </Grid>
//         {submitError && (
//           <Grid item xs={12}>
//             <FormHelperText error>{submitError}</FormHelperText>
//           </Grid>
//         )}
//         <Grid item xs={12}>
//           <Box sx={{ mt: 2 }}>
//             <Button
//               disabled={!isDirty}
//               fullWidth
//               size="large"
//               type="submit"
//               variant="contained"
//               color="secondary"
//               onClick={(e) => {
//                 e.stopPropagation();
//                 e.preventDefault();

//                 setIsDirty(false);
//                 if (validateForm()) {
//                   submitForm();
//                 }
//               }}
//             >
//               Sign up
//             </Button>
//           </Box>
//         </Grid>
//       </Grid>
//     </form>
//   );
// };
