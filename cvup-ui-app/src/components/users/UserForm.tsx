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
import { IIdName } from "../../models/AuthModels";
import {
  CrudTypesEnum,
  PermissionTypeEnum,
  TextValidateTypeEnum,
  UserActiveEnum,
} from "../../models/GeneralEnums";
import { IUser } from "../../models/AuthModels";
import {
  textFieldValidte,
  validateTxt,
  validteEmail,
  validtePhone,
} from "../../utils/Validation";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { enumToArrays } from "../../utils/GeneralUtils";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const UserForm = ({ onSaved, onCancel }: IProps) => {
  const { generalStore, authStore } = useStore();
  const [permissionsList, setPermissionsList] = useState<IIdName[]>([]);
  const [activationList, setActivationList] = useState<IIdName[]>([]);
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<IUser>({
    id: 0,
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    permissionType: PermissionTypeEnum.User,
    activeStatus: UserActiveEnum.Not_Active,
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
  });

  useEffect(() => {
    setPermissionsList(enumToArrays(PermissionTypeEnum));
    setActivationList(enumToArrays(UserActiveEnum));

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

    err = validteEmail(formModel.email, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.emailValid,
    ]);
    isFormValid = updateFieldError("email", err) && isFormValid;

    err = validtePhone(formModel.phone, [TextValidateTypeEnum.phoneValid]);
    isFormValid = updateFieldError("phone", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (validateForm()) {
      let response;

      if (crudType === CrudTypesEnum.Insert) {
        response = await authStore.addUser(formModel);
      } else {
        response = await authStore.updateUser(formModel);
      }

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deleteRecord = async () => {
    const isDelete = await generalStore.confirmDialog(
      "Delete User",
      "Are you sure you want to delete this user?"
    );

    if (isDelete) {
      const response = await authStore.deleteUser(formModel.id);

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={6}>
          <TextField
            fullWidth
            required
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
              clearError("lastName");
              setIsDirty(true);
            }}
            error={errModel.lastName !== ""}
            helperText={errModel.lastName}
            value={formModel.lastName}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            required
            InputLabelProps={{ shrink: true }}
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
            sx={{ minWidth: 350 }}
            fullWidth
            disabled={crudType === CrudTypesEnum.Delete}
            margin="normal"
            type="text"
            id="emailInp"
            label="Phone"
            variant="outlined"
            onChange={(e) => {
              setFormModel((currentProps) => ({
                ...currentProps,
                phone: e.target.value,
              }));
              clearError("phone");
              setIsDirty(true);
            }}
            error={errModel.phone !== ""}
            helperText={errModel.phone}
            value={formModel.phone}
          />
        </Grid>

        <Grid item xs={6}>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel id="permissionlabel">Permission Type</InputLabel>
            <Select
              disabled={crudType === CrudTypesEnum.Delete}
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
        <Grid item xs={6}>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel id="permissionlabel">Activation</InputLabel>
            <Select
              disabled={crudType === CrudTypesEnum.Delete}
              labelId="activationlabel"
              id="sctivationSelect"
              label="Activation"
              onChange={(e) => {
                setFormModel((currentProps) => ({
                  ...currentProps,
                  activeStatus: parseInt(e.target.value),
                }));
                updateFieldError("activeStatus", "");
              }}
              value={formModel.activeStatus.toString()}
            >
              {activationList.map((item) => {
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
        <Grid item xs={12} mt={3}>
          <Grid container justifyContent="space-between">
            <Grid item>
              {crudType === CrudTypesEnum.Update && (
                <Button
                  fullWidth
                  color="warning"
                  onClick={() => {
                    deleteRecord();
                  }}
                >
                  Delete
                </Button>
              )}
            </Grid>
            <Grid item>
              <Stack direction="row" alignItems="center" gap={1}>
                <Button fullWidth color="secondary" onClick={() => onCancel()}>
                  Cancel
                </Button>

                <Button
                  disabled={!isDirty}
                  fullWidth
                  color="secondary"
                  onClick={handleSubmit}
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
