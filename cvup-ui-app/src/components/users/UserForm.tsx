import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IUser } from "../../models/AuthModels";
import { textFieldValidte } from "../../utils/Validation";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const UserForm = ({ onSaved, onCancel }: IProps) => {
  const { generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [contactModel, setContactModel] = useState<IUser>({
    id: 0,
    firstName: "",
    lastName: "",
    companyName: "",
    companyId: 0,
    email: "",
    phone: "",
    password: "",
  });
  const [errModel, setErrModel] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
  });

  // const updateFieldError = (field: string, errTxt: string) => {
  //   const isValid = errTxt === "" ? true : false;
  //   setIsDirty(true);
  //   setSubmitError("");

  //   setFormValErrorTxt((currentProps) => ({
  //     ...currentProps,
  //     [field]: errTxt,
  //   }));
  //   setFormValError((currentProps) => ({
  //     ...currentProps,
  //     [field]: isValid === false,
  //   }));

  //   return isValid;
  // };

  const validateForm = () => {
    let isFormValid = true;
    // let errTxt = textFieldValidte(contactModel.name, true, true, true);
    // isFormValid = updateFieldError("name", errTxt) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    // const response = await generalStore.addUpdateHrCompany(contactModel);
    // if (response.isSuccess) {
    //   onSaved();
    // } else {
    //   return setSubmitError("An Error Occurred Please Try Again Later.");
    // }
  };

  const deleteRecord = async () => {
    // const response = await generalStore.deleteHrCompany(contactModel.id);
    // if (response.isSuccess) {
    //   onSaved();
    // } else {
    //   return setSubmitError("An Error Occurred Please Try Again Later.");
    // }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            margin="normal"
            type="text"
            id="title"
            label="Contact Name"
            variant="outlined"
            onChange={(e) => {
              setContactModel((currentProps) => ({
                ...currentProps,
                firstName: e.target.value,
              }));
              setErrModel((value) => ({
                ...value,
                firstName: "",
              }));
              setIsDirty(true);
            }}
            error={errModel.firstName !== ""}
            helperText={errModel.firstName}
            value={contactModel.firstName}
          />
        </Grid>

        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            margin="normal"
            type="text"
            id="title"
            label="Contact Name"
            variant="outlined"
            onChange={(e) => {
              setContactModel((currentProps) => ({
                ...currentProps,
                email: e.target.value,
              }));
              setErrModel((value) => ({
                ...value,
                email: "",
              }));
              setIsDirty(true);
            }}
            error={errModel.email !== ""}
            helperText={errModel.email}
            value={contactModel.email}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            margin="normal"
            type="text"
            id="title"
            label="Contact Name"
            variant="outlined"
            onChange={(e) => {
              setContactModel((currentProps) => ({
                ...currentProps,
                phone: e.target.value,
              }));
              setErrModel((value) => ({
                ...value,
                phone: "",
              }));
              setIsDirty(true);
            }}
            error={errModel.phone !== ""}
            helperText={errModel.phone}
            value={contactModel.phone}
          />
        </Grid>

        <Grid item xs={12}>
          <FormHelperText error>{submitError}</FormHelperText>
        </Grid>
        <Grid item xs={12} mt={2}>
          <Grid container justifyContent="flex-end">
            <Grid item>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button
                  fullWidth
                  variant="contained"
                  color="secondary"
                  onClick={() => onCancel()}
                >
                  Cancel
                </Button>
                <Button
                  fullWidth
                  variant="contained"
                  color="warning"
                  onClick={() => {
                    deleteRecord();
                  }}
                >
                  Delete
                </Button>
                <Button
                  disabled={!isDirty}
                  fullWidth
                  variant="contained"
                  color="secondary"
                  onClick={() => {
                    setIsDirty(false);
                    if (validateForm()) {
                      submitForm();
                    }
                  }}
                >
                  Save
                </Button>
              </Stack>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};
