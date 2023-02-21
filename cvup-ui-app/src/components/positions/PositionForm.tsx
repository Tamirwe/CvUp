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
import { useStateForm } from "../../Hooks/useStateForm";
import { useStore } from "../../Hooks/useStore";
import { IPosition } from "../../models/GeneralModels";
import { textFieldValidte } from "../../utils/Validation";
import { CustomersListDialog } from "../customers/CustomersListDialog";
import { HrCompaniesListDialog } from "../hrCompanies/HrCompaniesListDialog";
import { InterviewersListDialog } from "../interviewers/InterviewersListDialog";

export const PositionForm = observer(() => {
  let { pid } = useParams();
  const navigate = useNavigate();
  const { positionsStore, generalStore, authStore } = useStore();
  const [openCustomersList, setOpenCustomersList] = useState(false);
  const [openHrCompaniesList, setOpenHrCompaniesList] = useState(false);
  const [openInterviewersList, setOpenInterviewersList] = useState(false);
  const [isRtlDirection, setIsRtlDirection] = useState(false);
  const [hrCompanyNames, setHrCompanyNames] = useState<string[]>([]);
  const [interviewersNames, setInterviewersNames] = useState<string[]>([]);
  const [
    frmState,
    setFrmState,
    frmErrs,
    frmErrsMsgs,
    frmMsg,
    setFrmMsg,
    setFieldErr,
    setIsDirty,
    isDirty,
  ] = useStateForm<IPosition, any, any>(
    positionsStore.posSelected as IPosition,
    {
      name: false,
      descr: false,
    },
    {
      name: "",
      descr: "",
    }
  );

  useEffect(() => {
    (async () => {
      if (pid && parseInt(pid) > 0) {
        await positionsStore.getPosition(parseInt(pid));
      } else {
        positionsStore.newPosition();
      }

      setFrmState((currentProps) => ({
        ...currentProps,
        ...positionsStore.posSelected,
      }));
      setFrmMsg("");
      setIsDirty(false);
    })();
  }, [pid]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    (async () => {
      await Promise.all([
        generalStore.getCustomersList(false),
        generalStore.getHrCompaniesList(false),
        authStore.getInterviewersList(false),
      ]);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (frmState) {
      const interviewrsNamesArr: string[] = [];

      frmState.interviewersIds.forEach((id) => {
        const interviewer = authStore.interviewersList?.find(
          (x) => x.id === id
        );
        interviewrsNamesArr.push(
          `${interviewer?.firstName} ${interviewer?.lastName}` || ""
        );
      });

      setInterviewersNames(interviewrsNamesArr);
    }
  }, [frmState?.interviewersIds]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (frmState) {
      const hrCompaniesNamesArr: string[] = [];

      frmState.hrCompaniesIds.forEach((id) => {
        const hrCompany = generalStore.hrCompaniesList?.find(
          (x) => x.id === id
        );
        hrCompaniesNamesArr.push(hrCompany?.name || "");
      });

      setHrCompanyNames(hrCompaniesNamesArr);
    }
  }, [frmState?.hrCompaniesIds]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if (frmState) {
      const interviewrsNamesArr: string[] = [];

      frmState.interviewersIds.forEach((id) => {
        const interviewer = authStore.interviewersList?.find(
          (x) => x.id === id
        );
        interviewrsNamesArr.push(
          `${interviewer?.firstName} ${interviewer?.lastName}` || ""
        );
      });

      setInterviewersNames(interviewrsNamesArr);
    }
  }, [frmState?.interviewersIds]); // eslint-disable-line react-hooks/exhaustive-deps

  const handleHrCompaniesChanged = (
    event: SelectChangeEvent<typeof hrCompanyNames>,
    node: any
  ) => {
    const {
      target: { value },
    } = event;

    const id = parseInt(node.props.id);
    const isChecked = node.props.children[0].props.checked;
    const selectedCompanyIds = [...frmState.hrCompaniesIds];

    if (isChecked) {
      const ind = selectedCompanyIds.indexOf(id);
      selectedCompanyIds.splice(ind, 1);
    } else {
      selectedCompanyIds.push(id);
    }

    setIsDirty(true);

    setFrmState((currentProps) => ({
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
    const selectedInterviwersIds = [...frmState.interviewersIds];

    if (isChecked) {
      const ind = selectedInterviwersIds.indexOf(id);
      selectedInterviwersIds.splice(ind, 1);
    } else {
      selectedInterviwersIds.push(id);
    }

    setIsDirty(true);

    setFrmState((currentProps) => ({
      ...currentProps,
      interviewersIds: selectedInterviwersIds,
    }));

    setInterviewersNames(typeof value === "string" ? value.split(",") : value);
  };

  const handleCustomersListClose = () => {
    setOpenCustomersList(false);
  };

  const handleHrCompaniesListClose = () => {
    setOpenHrCompaniesList(false);
  };

  const handleInterviewersListClose = () => {
    setOpenInterviewersList(false);
  };

  const validateForm = () => {
    let isFormValid = true;
    let errTxt = textFieldValidte(frmState.name, true, true, true);
    isFormValid = setFieldErr("name", errTxt) && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await positionsStore.addUpdatePosition(frmState);

    if (response.isSuccess) {
      positionsStore.getPositionsList(true);
      navigate(`/position/${response.data}`);
    } else {
      return setFrmMsg("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <>
      {frmState && (
        <form noValidate spellCheck="false">
          <Grid container spacing={2}>
            <Grid item xs={12} lg={12}>
              <Grid container>
                <Grid item xs={12}>
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    spacing={1}
                  >
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
                        setIsDirty(true);

                        setFrmState((currentProps) => ({
                          ...currentProps,
                          name: e.target.value,
                        }));
                        setFieldErr("name", "");
                      }}
                      error={frmErrs.name}
                      helperText={frmErrsMsgs.name}
                      value={frmState.name}
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
                      setIsDirty(true);

                      setFrmState((currentProps) => ({
                        ...currentProps,
                        descr: e.target.value,
                      }));
                      setFieldErr("descr", "");
                    }}
                    error={frmErrs.descr}
                    helperText={frmErrsMsgs.descr}
                    value={frmState.descr}
                  />
                </Grid>
              </Grid>
            </Grid>

            <Grid item xs={12} lg={12} sx={{ mt: 2 }}>
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
                                frmState.interviewersIds.indexOf(item.id) > -1
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
                        checked={frmState.isActive}
                        onChange={(e) => {
                          setIsDirty(true);

                          setFrmState((currentProps) => ({
                            ...currentProps,
                            isActive: e.target.checked,
                          }));
                        }}
                        inputProps={{ "aria-label": "controlled" }}
                      />
                    }
                    label={frmState.isActive ? "Active" : "Not Active"}
                  />
                </Grid>

                <Grid item xs={12} lg={6}>
                  <FormControl fullWidth>
                    <InputLabel id="customerLabel">Customer</InputLabel>
                    <Select
                      labelId="customerLabel"
                      id="customerSelect"
                      value={
                        frmState.customerId === 0
                          ? ""
                          : frmState.customerId.toString()
                      }
                      label="Customer"
                      onChange={(event: SelectChangeEvent) => {
                        setIsDirty(true);

                        setFrmState((currentProps) => ({
                          ...currentProps,
                          customerId: parseInt(event.target.value),
                        }));
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
                      {generalStore.customersList?.map((item, i) => {
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
                              checked={
                                frmState.hrCompaniesIds.indexOf(item.id) > -1
                              }
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
            {frmMsg && (
              <Grid item xs={12}>
                <FormHelperText error>{frmMsg}</FormHelperText>
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
              close={handleCustomersListClose}
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
      )}
    </>
  );
});
