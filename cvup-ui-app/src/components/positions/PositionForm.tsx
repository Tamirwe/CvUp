import {
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
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdFormatIndentIncrease } from "react-icons/md";
import { useParams } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { IPosition } from "../../models/GeneralModels";
import { textFieldValidte } from "../../utils/Validation";
import { DepartmentsListDialog } from "../departments/DepartmentsListDialog";
import { HrCompaniesListDialog } from "../hrCompanies/HrCompaniesListDialog";
import { InterviewersListDialog } from "../interviewers/InterviewersListDialog";

// interface IProps {
//   interviewer: IInterviewer;
// }

export const PositionForm = observer(() => {
  let { pid } = useParams();
  const { positionsStore, generalStore, authStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [openDepartmentsList, setOpenDepartmentsList] = useState(false);
  const [openHrCompaniesList, setOpenHrCompaniesList] = useState(false);
  const [openInterviewersList, setOpenInterviewersList] = useState(false);
  const [hrCompanyNames, setHrCompanyNames] = useState<string[]>([]);
  const [interviewersNames, setInterviewersNames] = useState<string[]>([]);
  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<IPosition>({
    id: 0,
    name: "",
    descr: "",
    isActive: true,
    departmentId: 0,
    hrCompaniesIds: [],
    interviewersIds: [],
  });
  const [formValError, setFormValError] = useState({
    name: false,
    descr: false,
  });
  const [formValErrorTxt, setFormValErrorTxt] = useState({
    name: "",
    descr: "",
  });

  useEffect(() => {
    (async () => {
      await Promise.all([
        pid && positionsStore.GetPosition(parseInt(pid)),
        generalStore.getDepartmentsList(false),
        generalStore.getHrCompaniesList(false),
        authStore.getInterviewersList(false),
      ]);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (positionsStore.position) {
      setFormModel(positionsStore.position);
    }
  }, [positionsStore.position]);

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

    setFormModel((currentProps) => ({
      ...currentProps,
      interviewersIds: selectedInterviwersIds,
    }));

    setInterviewersNames(typeof value === "string" ? value.split(",") : value);
  };

  const handleDepartmentsListClose = () => {
    setOpenDepartmentsList(false);
  };

  const handleHrCompaniesListClose = () => {
    setOpenHrCompaniesList(false);

    const hrCompaniesNamesArr: string[] = [];

    formModel.hrCompaniesIds.forEach((id) => {
      const hrCompany = generalStore.hrCompaniesList?.find((x) => x.id === id);
      hrCompaniesNamesArr.push(hrCompany?.name || "");
    });

    setHrCompanyNames(hrCompaniesNamesArr);
  };

  const handleInterviewersListClose = () => {
    setOpenInterviewersList(false);

    const interviewrsNamesArr: string[] = [];

    formModel.interviewersIds.forEach((id) => {
      const interviewer = authStore.interviewersList?.find((x) => x.id === id);
      interviewrsNamesArr.push(
        `${interviewer?.firstName} ${interviewer?.lastName}` || ""
      );
    });

    setInterviewersNames(interviewrsNamesArr);
  };

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
    let errTxt = textFieldValidte(formModel.name, true, true, true);
    isFormValid = updateFieldError("name", errTxt) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await positionsStore.addUpdatePosition(formModel);

    if (response.isSuccess) {
      positionsStore.getPositionsList(true);
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container spacing={2}>
        <Grid item xs={12} lg={6}>
          <Grid container>
            <Grid item xs={12}>
              <TextField
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
                  updateFieldError("name", "");
                }}
                error={formValError.name}
                helperText={formValErrorTxt.name}
                value={formModel.name}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
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
                  updateFieldError("descr", "");
                }}
                error={formValError.descr}
                helperText={formValErrorTxt.descr}
                value={formModel.descr}
              />
            </Grid>
          </Grid>
        </Grid>

        <Grid item xs={12} lg={6} sx={{ mt: 2 }}>
          <Grid container spacing={3}>
            <Grid item xs={12} lg={6}>
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
                  sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                  endAdornment={
                    <IconButton
                      aria-label="toggle password visibility"
                      onClick={() => setOpenInterviewersList(true)}
                      edge="end"
                    >
                      <MdFormatIndentIncrease />
                    </IconButton>
                  }
                >
                  {authStore.interviewersList?.map((item, i) => {
                    return (
                      <MenuItem
                        key={item.id}
                        id={item.id.toString()}
                        value={`${item.firstName} ${item.lastName}`}
                      >
                        <Checkbox
                          checked={
                            interviewersNames.indexOf(
                              `${item.firstName} ${item.lastName}`
                            ) > -1
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
            <Grid item xs={12} lg={6} mt={1}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formModel.isActive}
                    onChange={(e) =>
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        isActive: e.target.checked,
                      }))
                    }
                    inputProps={{ "aria-label": "controlled" }}
                  />
                }
                label={formModel.isActive ? "Active" : "Not Active"}
              />
            </Grid>

            <Grid item xs={12} lg={6}>
              <FormControl fullWidth>
                <InputLabel id="departmentLabel">Department</InputLabel>
                <Select
                  labelId="departmentLabel"
                  id="departmentSelect"
                  value={
                    formModel.departmentId === 0
                      ? ""
                      : formModel.departmentId.toString()
                  }
                  label="Department"
                  onChange={(event: SelectChangeEvent) => {
                    setFormModel((currentProps) => ({
                      ...currentProps,
                      departmentId: parseInt(event.target.value),
                    }));
                  }}
                  sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                  endAdornment={
                    <IconButton
                      aria-label="toggle password visibility"
                      onClick={() => setOpenDepartmentsList(true)}
                      edge="end"
                    >
                      <MdFormatIndentIncrease />
                    </IconButton>
                  }
                >
                  {generalStore.departmentsList?.map((item, i) => {
                    return (
                      <MenuItem key={item.id} value={item.id}>
                        {item.name}
                      </MenuItem>
                    );
                  })}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} lg={6}>
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
                          checked={hrCompanyNames.indexOf(item.name) > -1}
                        />
                        <ListItemText primary={item.name} />
                      </MenuItem>
                    );
                  })}
                </Select>
              </FormControl>
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
      {openDepartmentsList && (
        <DepartmentsListDialog
          isOpen={openDepartmentsList}
          close={handleDepartmentsListClose}
        />
      )}
      {openHrCompaniesList && (
        <HrCompaniesListDialog
          isOpen={openHrCompaniesList}
          close={handleHrCompaniesListClose}
        />
      )}
      {openInterviewersList && (
        <InterviewersListDialog
          isOpen={openInterviewersList}
          close={handleInterviewersListClose}
        />
      )}
    </form>
  );
});
