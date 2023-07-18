import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
  Tooltip,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdFormatAlignRight, MdFormatAlignLeft } from "react-icons/md";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  CrudTypesEnum,
  PositionStatusEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { IPosition } from "../../models/GeneralModels";
import { validateTxt } from "../../utils/Validation";
import { ContactsAutoCompleteMulty } from "./ContactsAutoCompleteMulty";
import { UsersAutoCompleteMulty } from "./UsersAutoCompleteMulty";
import { format } from "date-fns";

interface IProps {
  onSaved: (id: number) => void;
  onCancel: () => void;
}

export const PositionForm = observer(({ onSaved, onCancel }: IProps) => {
  const { positionsStore, authStore, generalStore, customersContactsStore } =
    useStore();
  const [isRtlDirection, setIsRtlDirection] = useState(false);
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);

  const [submitError, setSubmitError] = useState("");
  const [isDirty, setIsDirty] = useState(false);
  const [formModel, setFormModel] = useState<IPosition>({
    id: 0,
    name: "",
    descr: "",
    requirements: "",
    updated: new Date(),
    status: PositionStatusEnum.Active,
    customerId: 0,
    customerName: "",
    interviewersIds: [],
    contactsIds: [],
    emailsubjectAddon: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
  });

  useEffect(() => {
    if (positionsStore.editPosition) {
      setCrudType(CrudTypesEnum.Update);
      setFormModel({ ...positionsStore.editPosition });
    }

    (async () => {
      await Promise.all([
        authStore.usersList.length === 0 && authStore.getUsersList(),
        customersContactsStore.contactsListSorted.length === 0 &&
          customersContactsStore.getContactsList(),
      ]);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (formModel) {
      const interviewrsNamesArr: string[] = [];

      formModel.interviewersIds.forEach((id) => {
        const interviewer = authStore.interviewersList?.find(
          (x) => x.id === id
        );
        interviewrsNamesArr.push(
          `${interviewer?.firstName} ${interviewer?.lastName}` || ""
        );
      });
    }
  }, [formModel?.interviewersIds]); // eslint-disable-line react-hooks/exhaustive-deps

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.name, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("name", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (validateForm()) {
      let response;

      if (crudType === CrudTypesEnum.Insert) {
        response = await positionsStore.addPosition(formModel);
      } else {
        response = await positionsStore.updatePosition(formModel);
      }

      if (response.isSuccess) {
        onSaved(response.data);
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deletePosition = async () => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete User",
      "Are you sure you want to delete this user?"
    );

    if (isDelete) {
      const response = await positionsStore.deletePosition(formModel.id);

      if (response.isSuccess) {
        onSaved(0);
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const activatePosition = async () => {
    const response = await positionsStore.activatePosition(formModel);

    if (response.isSuccess) {
      onSaved(formModel.id);
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  const dactivatePosition = async () => {
    const response = await positionsStore.dactivatePosition(formModel);

    if (response.isSuccess) {
      await generalStore.alertConfirmDialog(
        AlertConfirmDialogEnum.Confirm,
        "Deactivate Position",
        "Position Deactivated."
      );

      onSaved(formModel.id);
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <>
      {formModel && (
        <form noValidate spellCheck="false">
          <Grid container>
            <Grid item xs={12} lg={12} sx={{ direction: "rtl" }}>
              <Grid container>
                <Grid item xs={12} lg={8}>
                  <TextField
                    sx={{
                      direction: "rtl",
                    }}
                    fullWidth
                    required
                    margin="normal"
                    type="text"
                    id="title"
                    label="Position title"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        name: e.target.value,
                      }));
                      clearError("firstName");
                      setIsDirty(true);
                    }}
                    error={errModel.name !== ""}
                    helperText={errModel.name}
                    value={formModel.name}
                  />
                </Grid>
                <Grid item xs={12} lg={2}>
                  <FormControl fullWidth sx={{ mt: 2 }}>
                    <InputLabel id="permissionlabel">status</InputLabel>
                    <Select
                      sx={{ direction: "ltr" }}
                      disabled={crudType === CrudTypesEnum.Delete}
                      labelId="permissionlabel"
                      id="permissionTypeSelect"
                      label="status"
                      onChange={(e) => {
                        setFormModel((currentProps) => ({
                          ...currentProps,
                          status: e.target.value as PositionStatusEnum,
                        }));
                        setIsDirty(true);
                      }}
                      value={formModel.status}
                    >
                      {Object.keys(PositionStatusEnum)
                        .filter((x) => isNaN(Number(x)))
                        .map((key, i) => {
                          return (
                            <MenuItem key={i} value={i}>
                              {key.replace("_", " ")}
                            </MenuItem>
                          );
                        })}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12} lg={2}>
                  <TextField
                    sx={{
                      direction: "rtl",
                    }}
                    fullWidth
                    disabled={true}
                    margin="normal"
                    type="text"
                    id="lastupdate"
                    label="Last update"
                    variant="outlined"
                    value={
                      format(new Date(formModel.updated), "MMM d, yyyy") || ""
                    }
                  />
                </Grid>
                <Grid item xs={12} lg={8} mt={2} sx={{ direction: "ltr" }}>
                  <ContactsAutoCompleteMulty
                    options={customersContactsStore.contactsListSorted}
                    valueIds={formModel.contactsIds}
                    onChange={(value, customerId) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        contactsIds: value,
                      }));
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        customerId: customerId,
                      }));
                      setIsDirty(true);
                    }}
                  />
                </Grid>

                <Grid item xs={12} lg={4}>
                  <TextField
                    sx={{
                      direction: "rtl",
                    }}
                    fullWidth
                    required
                    margin="normal"
                    type="text"
                    id="title"
                    label="Email Subject add-on"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        emailsubjectAddon: e.target.value,
                      }));
                      clearError("firstName");
                      setIsDirty(true);
                    }}
                    value={formModel.emailsubjectAddon || ""}
                  />
                </Grid>

                <Grid item xs={12} lg={6}>
                  <TextField
                    sx={{
                      direction: "rtl",
                      // "& textarea": {
                      //   direction: isRtlDirection ? "rtl" : "ltr",
                      // },
                    }}
                    fullWidth
                    multiline
                    rows={18}
                    margin="normal"
                    type="text"
                    id="description"
                    label="Description"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        descr: e.target.value,
                      }));
                      setIsDirty(true);
                    }}
                    value={formModel.descr}
                  />
                </Grid>
                <Grid item xs={12} lg={6}>
                  <TextField
                    sx={{
                      direction: "rtl",
                      // "& textarea": {
                      //   direction: isRtlDirection ? "rtl" : "ltr",
                      // },
                    }}
                    fullWidth
                    multiline
                    rows={18}
                    margin="normal"
                    type="text"
                    id="requirements"
                    label="Requirements"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        requirements: e.target.value,
                      }));
                      setIsDirty(true);
                    }}
                    value={formModel.requirements}
                  />
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <Grid container>
                {/* {generalStore.appMode === AppModeEnum.HRCompany ? ( */}

                {/* ) : ( */}

                {/* )} */}
                <Grid item xs={12} mt={3}></Grid>
              </Grid>
            </Grid>
            {submitError && (
              <Grid item xs={12}>
                <div style={{ direction: "ltr" }}>
                  <FormHelperText error>{submitError}</FormHelperText>
                </div>
              </Grid>
            )}
            <Grid item xs={12} mt={4}>
              <Grid container justifyContent="space-between">
                <Grid item>
                  {crudType === CrudTypesEnum.Update &&
                    authStore.claims.UserId &&
                    parseInt(authStore.claims.UserId) !== formModel.id && (
                      <Stack direction="row" alignItems="center" gap={1}>
                        <Button
                          fullWidth
                          color="error"
                          onClick={() => {
                            deletePosition();
                          }}
                        >
                          Delete
                        </Button>
                        <Button
                          fullWidth
                          color="warning"
                          onClick={() => {
                            if (
                              formModel.status === PositionStatusEnum.Not_Active
                            ) {
                              activatePosition();
                            } else {
                              dactivatePosition();
                            }
                          }}
                        >
                          {formModel.status === PositionStatusEnum.Not_Active
                            ? "Activate"
                            : "Deactivate"}
                        </Button>
                      </Stack>
                    )}
                </Grid>
                <Grid item>
                  <Stack direction="row" alignItems="center" gap={1}>
                    <Button
                      fullWidth
                      color="secondary"
                      onClick={() => onCancel()}
                    >
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
            {/* <Grid item xs={12} lg={10} mt={2}>
                  <UsersAutoCompleteMulty
                    options={authStore.usersList}
                    valueIds={formModel.interviewersIds}
                    onChange={(value) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        interviewersIds: value,
                      }));
                      setIsDirty(true);
                    }}
                  />
                </Grid> */}
            {/* <Grid item xs={12} sx={{ mt: 2 }}>
              <Grid container>
                <Grid item xs={0} lg={10}></Grid>
                <Grid item xs={12} lg={2}>
                  <Stack direction="row" alignItems="center" gap={1}>
                    <Button
                      disabled={!isDirty}
                      fullWidth
                      size="large"
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
                      Cancel
                    </Button>
                    <Button
                      disabled={!isDirty}
                      fullWidth
                      size="large"
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
                      Save
                    </Button>
                  </Stack>
                </Grid>
              </Grid>
            </Grid> */}
          </Grid>
        </form>
      )}
    </>
  );
});
