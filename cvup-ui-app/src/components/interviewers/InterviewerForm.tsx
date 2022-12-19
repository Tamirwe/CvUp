import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName, IInterviewer } from "../../models/AuthModels";
import { CrudTypesEnum, PermissionTypeEnum } from "../../models/GeneralEnums";
import { enumToArrays } from "../../utils/GeneralUtils";
import { textFieldValidte } from "../../utils/Validation";

interface IProps {
  interviewer: IInterviewer;
  crudType?: CrudTypesEnum;
  onSaved: () => void;
  onCancel: () => void;
}

export const InterviewerForm = ({
  interviewer,
  crudType,
  onSaved,
  onCancel,
}: IProps) => {
  const { authStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [permissionsList, setPermissionsList] = useState<IIdName[]>([]);
  const [formModel, setFormModel] = useState<IInterviewer>(interviewer);
  const [formValError, setFormValError] = useState({
    firstName: false,
    lastName: false,
    email: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    firstName: "",
    lastName: "",
    email: "",
  });

  useEffect(() => {
    setPermissionsList(enumToArrays(PermissionTypeEnum));
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    interviewer && setFormModel({ ...interviewer });
  }, [interviewer]);

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
    let isFormValid = true;
    let errTxt = textFieldValidte(formModel.firstName, true, true, true);
    isFormValid = updateFieldError("firstName", errTxt) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await authStore.addUpdateInterviewer(formModel);

    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  const deleteRecord = async () => {
    const response = await authStore.deleteInterviewer(formModel);

    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={6}>
          <TextField
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
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
              updateFieldError("firstName", "");
            }}
            error={formValError.firstName}
            helperText={formValErrorTxt.firstName}
            value={formModel.firstName}
          />
        </Grid>
        <Grid item xs={6}>
          <TextField
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
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
              updateFieldError("lastName", "");
            }}
            error={formValError.lastName}
            helperText={formValErrorTxt.lastName}
            value={formModel.lastName}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="emailInp"
            label="Email"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                email: e.target.value,
              }));
              updateFieldError("email", "");
            }}
            error={formValError.email}
            helperText={formValErrorTxt.email}
            value={formModel.email}
          />
        </Grid>
        <Grid item xs={12}>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel id="permissionlabel">Permission Type</InputLabel>
            <Select
              labelId="permissionlabel"
              id="permissionTypeSelect"
              label="Permission Type"
              onChange={(e) => {
                setFormModel((currentProps) => ({
                  ...currentProps,
                  permissionType: parseInt(e.target.value),
                }));
                updateFieldError("permissionType", "");
              }}
              value={formModel.permissionType.toString()}
            >
              {permissionsList.map((item) => {
                // console.log(key, index);
                return (
                  <MenuItem key={item.id} value={item.id}>
                    {item.name}
                  </MenuItem>
                );
              })}
            </Select>
          </FormControl>
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
                {crudType === CrudTypesEnum.Delete ? (
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
                ) : (
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
                )}
              </Stack>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};
