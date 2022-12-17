import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  Stack,
  TextField,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdFormatIndentIncrease, MdOutlineVisibility } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { IPosition } from "../../models/AuthModels";
import { textFieldValidte } from "../../utils/Validation";
import { DepartmentListDialog } from "../departments/DepartmentListDialog";

export const PositionForm = observer(() => {
  const { positionsStore, generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [openDepartments, setOpenDepartments] = useState(false);
  const [department, setDepartment] = useState("");

  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<IPosition>({
    id: 0,
    name: "",
    descr: "",
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
    generalStore.getCompanyDepartments(false);
  }, []);

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
      if (response.data === "duplicateUserPass") {
        return setSubmitError("Duplcate User Name and Password.");
      }
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
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
        <Grid item xs={12} lg={6}>
          <Grid container>
            <Grid item xs={12} lg={6}>
              <FormControl fullWidth>
                <InputLabel id="departmentLabel">Department</InputLabel>
                <Select
                  labelId="departmentLabel"
                  id="demo-simple-select"
                  value={department}
                  label="Department"
                  onChange={(event: SelectChangeEvent) => {
                    setDepartment(event.target.value);
                  }}
                  sx={{ "& .MuiSelect-icon": { right: "45px !important" } }}
                  endAdornment={
                    <IconButton
                      aria-label="toggle password visibility"
                      onClick={() => setOpenDepartments(true)}
                      edge="end"
                    >
                      <MdFormatIndentIncrease />
                    </IconButton>
                  }
                >
                  {generalStore.departmentsList?.map((item, i) => {
                    return <MenuItem value={item.id}>{item.name}</MenuItem>;
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
      {openDepartments && (
        <DepartmentListDialog
          isOpen={openDepartments}
          close={() => setOpenDepartments(false)}
        />
      )}
    </form>
  );
});
