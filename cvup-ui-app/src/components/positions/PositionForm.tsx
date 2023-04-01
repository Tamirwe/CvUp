import {
  Autocomplete,
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputLabel,
  ListItemText,
  MenuItem,
  OutlinedInput,
  Select,
  SelectChangeEvent,
  Stack,
  Switch,
  TextField,
  Tooltip,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import {
  MdFormatIndentIncrease,
  MdFormatAlignRight,
  MdFormatAlignLeft,
} from "react-icons/md";
import { useNavigate, useParams } from "react-router-dom";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStateForm } from "../../Hooks/useStateForm";
import { useStore } from "../../Hooks/useStore";
import { AppModeEnum, CrudTypesEnum } from "../../models/GeneralEnums";
import { IPosition } from "../../models/GeneralModels";
import { textFieldValidte } from "../../utils/Validation";
import { ContactsAutoCompleteMulty } from "../contacts/ContactsAutoCompleteMulty";
import { CustomersListDialog } from "../customers/CustomersListDialog";
import { HrCompaniesListDialog } from "../hrCompanies/HrCompaniesListDialog";
import { InterviewersListDialog } from "../interviewers/InterviewersListDialog";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const PositionForm = observer(({ onSaved, onCancel }: IProps) => {
  let { pid } = useParams();
  const navigate = useNavigate();
  const { positionsStore, generalStore, authStore, customersContactsStore } =
    useStore();
  const [openCustomersList, setOpenCustomersList] = useState(false);
  const [openHrCompaniesList, setOpenHrCompaniesList] = useState(false);
  // const [openInterviewersList, setOpenInterviewersList] = useState(false);
  const [isRtlDirection, setIsRtlDirection] = useState(false);
  const [hrCompanyNames, setHrCompanyNames] = useState<string[]>([]);
  const [interviewersNames, setInterviewersNames] = useState<string[]>([]);

  const [submitError, setSubmitError] = useState("");
  const [isDirty, setIsDirty] = useState(false);
  const [formModel, setFormModel] = useState<IPosition>({
    id: 0,
    name: "",
    descr: "",
    isActive: true,
    customerId: 0,
    hrCompaniesIds: [],
    interviewersIds: [],
    contactsIds: [],
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
    descr: "",
  });

  // useEffect(() => {
  //   (async () => {
  //     if (pid && parseInt(pid) > 0) {
  //       await positionsStore.getPosition(parseInt(pid));
  //     } else {
  //       positionsStore.newPosition();
  //     }

  //     setFormModel((currentProps) => ({
  //       ...currentProps,
  //       ...positionsStore.selectedPosition,
  //     }));
  //     setSubmitError("");
  //     setIsDirty(false);
  //   })();
  // }, [pid]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    (async () => {
      await Promise.all([
        customersContactsStore.getCustomersList(),
        generalStore.getHrCompaniesList(false),
        authStore.usersList.length === 0 && authStore.getUsersList(),
        customersContactsStore.contactsList.length === 0 &&
          customersContactsStore.getContactsList,
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

      // setInterviewersNames(interviewrsNamesArr);
    }
  }, [formModel?.interviewersIds]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (formModel) {
      const hrCompaniesNamesArr: string[] = [];

      formModel.hrCompaniesIds.forEach((id) => {
        const hrCompany = generalStore.hrCompaniesList?.find(
          (x) => x.id === id
        );
        hrCompaniesNamesArr.push(hrCompany?.name || "");
      });

      setHrCompanyNames(hrCompaniesNamesArr);
    }
  }, [formModel?.hrCompaniesIds]); // eslint-disable-line react-hooks/exhaustive-deps

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

      // setInterviewersNames(interviewrsNamesArr);
    }
  }, [formModel?.interviewersIds]); // eslint-disable-line react-hooks/exhaustive-deps

  const handleHrCompaniesChanged = (
    event: SelectChangeEvent<typeof hrCompanyNames>,
    node: any
  ) => {
    const {
      target: { value },
    } = event;

    const id = parseInt(node.props.id);
    const isChecked = node.props.children[0].props.checked;
    const selectedCompanyIds = [...formModel.hrCompaniesIds];

    if (isChecked) {
      const ind = selectedCompanyIds.indexOf(id);
      selectedCompanyIds.splice(ind, 1);
    } else {
      selectedCompanyIds.push(id);
    }

    setIsDirty(true);

    setFormModel((currentProps) => ({
      ...currentProps,
      hrCompaniesIds: selectedCompanyIds,
    }));

    setHrCompanyNames(typeof value === "string" ? value.split(",") : value);
  };

  const handleInterviewersChanged = (
    event: SelectChangeEvent<typeof interviewersNames>,
    node: any
  ) => {
    const {
      target: { value },
    } = event;

    const id = parseInt(node.props.id);
    const isChecked = node.props.children[0].props.checked;
    const selectedInterviwersIds = [...formModel.interviewersIds];

    if (isChecked) {
      const ind = selectedInterviwersIds.indexOf(id);
      selectedInterviwersIds.splice(ind, 1);
    } else {
      selectedInterviwersIds.push(id);
    }

    setIsDirty(true);

    setFormModel((currentProps) => ({
      ...currentProps,
      interviewersIds: selectedInterviwersIds,
    }));

    if (value && Array.isArray(value)) {
      setInterviewersNames([...value]);
    }
  };

  const handleCustomersListClose = () => {
    setOpenCustomersList(false);
  };

  const handleHrCompaniesListClose = () => {
    setOpenHrCompaniesList(false);
  };

  // const handleInterviewersListClose = () => {
  //   setOpenInterviewersList(false);
  // };

  const validateForm = () => {
    let isFormValid = true;
    let err = textFieldValidte(formModel.name, true, true, true);
    isFormValid = updateFieldError("lastName", err) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await positionsStore.addUpdatePosition(formModel);

    if (response.isSuccess) {
      positionsStore.getPositionsList(true);
      navigate(`/position/${response.data}`);
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <>
      {formModel && (
        <form noValidate spellCheck="false">
          <Grid container>
            <Grid item xs={12} lg={12}>
              <Grid container>
                <Grid item xs={12}>
                  <Stack direction="row" justifyContent="space-between">
                    <TextField
                      sx={{
                        "& input": {
                          direction: isRtlDirection ? "rtl" : "ltr",
                        },
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
                    <Tooltip title="Direction">
                      <IconButton
                        sx={{
                          height: "fit-content",
                          alignSelf: "center",
                        }}
                        size="medium"
                        onClick={() => setIsRtlDirection(!isRtlDirection)}
                      >
                        {isRtlDirection ? (
                          <MdFormatAlignLeft />
                        ) : (
                          <MdFormatAlignRight />
                        )}
                      </IconButton>
                    </Tooltip>
                  </Stack>
                </Grid>
                <Grid item xs={12}>
                  <TextField
                    sx={{
                      "& textarea": {
                        direction: isRtlDirection ? "rtl" : "ltr",
                      },
                    }}
                    fullWidth
                    multiline
                    rows={8}
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
                      clearError("firstName");
                      setIsDirty(true);
                    }}
                    error={errModel.descr !== ""}
                    helperText={errModel.descr}
                    value={formModel.descr}
                  />
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <Grid container>
                <Grid item xs={12} mt={2}>
                  <ContactsAutoCompleteMulty
                    allRecordslist={customersContactsStore.contactsList}
                    selectedRecords={[]}
                  />
                </Grid>
                <Grid item xs={12} mt={3}>
                  <FormControl fullWidth>
                    <InputLabel id="interviewersLabel">Interviewers</InputLabel>
                    <Select
                      labelId="interviewersLabel"
                      id="interviewersSelect"
                      multiple
                      value={interviewersNames}
                      renderValue={(selected) => selected.join(", ")}
                      input={<OutlinedInput label="Interviewers" />}
                      onChange={handleInterviewersChanged}
                      // sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                      // endAdornment={
                      //   <IconButton
                      //     aria-label="toggle password visibility"
                      //     onClick={() => setOpenInterviewersList(true)}
                      //     edge="end"
                      //   >
                      //     <MdFormatIndentIncrease />
                      //   </IconButton>
                      // }
                    >
                      {authStore.usersList?.map((item, i) => {
                        return (
                          <MenuItem
                            key={item.id}
                            id={item.id.toString()}
                            value={`${item.firstName} ${item.lastName}`}
                          >
                            <Checkbox
                              checked={
                                formModel.interviewersIds.indexOf(item.id) > -1
                              }
                            />
                            <ListItemText
                              primary={`${item.firstName} ${item.lastName}`}
                            />
                          </MenuItem>
                        );
                      })}
                    </Select>
                  </FormControl>
                </Grid>

                <Grid item xs={6} mt={3}>
                  <FormControl fullWidth>
                    <InputLabel id="customerLabel">
                      {generalStore.appMode === AppModeEnum.HRCompany
                        ? "Customer"
                        : "Team"}
                    </InputLabel>
                    <Select
                      labelId="customerLabel"
                      id="customerSelect"
                      value={
                        formModel.customerId === 0
                          ? ""
                          : formModel.customerId.toString()
                      }
                      label="Customer"
                      onChange={(event: SelectChangeEvent) => {
                        setFormModel((currentProps) => ({
                          ...currentProps,
                          customerId: parseInt(event.target.value),
                        }));
                        setIsDirty(true);
                      }}
                      sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                      endAdornment={
                        <IconButton
                          aria-label="toggle password visibility"
                          onClick={() => setOpenCustomersList(true)}
                          edge="end"
                        >
                          <MdFormatIndentIncrease />
                        </IconButton>
                      }
                    >
                      {customersContactsStore.customersList?.map((item, i) => {
                        return (
                          <MenuItem key={item.id} value={item.id}>
                            {item.name}
                          </MenuItem>
                        );
                      })}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={6} mt={3}>
                  <FormControl fullWidth>
                    <InputLabel id="hrCompanyLabel">HR Companies</InputLabel>
                    <Select
                      labelId="hrCompanyLabel"
                      id="hrCompanySelect"
                      multiple
                      value={hrCompanyNames}
                      renderValue={(selected) => selected.join(", ")}
                      input={<OutlinedInput label="HR Companies" />}
                      onChange={handleHrCompaniesChanged}
                      sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                      endAdornment={
                        <IconButton
                          aria-label="toggle password visibility"
                          onClick={() => setOpenHrCompaniesList(true)}
                          edge="end"
                        >
                          <MdFormatIndentIncrease />
                        </IconButton>
                      }
                    >
                      {generalStore.hrCompaniesList?.map((item, i) => {
                        return (
                          <MenuItem
                            key={item.id}
                            id={item.id.toString()}
                            value={item.name}
                          >
                            <Checkbox
                              checked={
                                formModel.hrCompaniesIds.indexOf(item.id) > -1
                              }
                            />
                            <ListItemText primary={item.name} />
                          </MenuItem>
                        );
                      })}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12} mt={3}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formModel.isActive}
                        onChange={(e) => {
                          setFormModel((currentProps) => ({
                            ...currentProps,
                            isActive: e.target.checked,
                          }));
                          setIsDirty(true);
                        }}
                        inputProps={{ "aria-label": "controlled" }}
                      />
                    }
                    label={formModel.isActive ? "Active" : "Not Active"}
                  />
                </Grid>
              </Grid>
            </Grid>
            {submitError && (
              <Grid item xs={12}>
                <FormHelperText error>{submitError}</FormHelperText>
              </Grid>
            )}
            <Grid item xs={12} sx={{ mt: 2 }}>
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
            </Grid>
          </Grid>
          {openCustomersList && (
            <CustomersListDialog
              isOpen={openCustomersList}
              onClose={handleCustomersListClose}
            />
          )}
          {openHrCompaniesList && (
            <HrCompaniesListDialog
              isOpen={openHrCompaniesList}
              close={handleHrCompaniesListClose}
            />
          )}
          {/* {openInterviewersList && (
            <InterviewersListDialog
              isOpen={openInterviewersList}
              close={handleInterviewersListClose}
            />
          )} */}
        </form>
      )}
    </>
  );
});
