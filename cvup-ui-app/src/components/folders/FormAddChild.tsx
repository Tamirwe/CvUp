import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IFolder } from "../../models/GeneralModels";
import { textFieldValidte } from "../../utils/Validation";

interface IProps {
  folder?: IFolder;
  onSaved: () => void;
  onCancel: () => void;
}

export const FormAddChild = ({ folder, onSaved, onCancel }: IProps) => {
  const { foldersStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [folderModel, setFolderModel] = useState<IFolder>({
    id: 0,
    name: "",
    parentId: foldersStore.editFolderSelected?.id || 0,
  });
  const [errModel, setErrModel] = useState({
    name: "",
  });

  useEffect(() => {
    if (folder) {
      folder && setFolderModel({ ...folder });
    }
  }, [folder]);

  const validateForm = () => {
    let isFormValid = true;
    let errTxt = textFieldValidte(folderModel.name, true, true, true);
    setErrModel((value) => ({
      ...value,
      name: errTxt,
    }));

    isFormValid = errTxt === "" && isFormValid;
    return isFormValid;
  };

  const submitForm = async () => {
    const response = await foldersStore.addFolder(folderModel);
    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <form noValidate spellCheck="false">
      <Grid container>
        <Grid item xs={12} mb={2}>
          <TextField
            disabled
            sx={{ minWidth: 350 }}
            fullWidth
            margin="normal"
            type="text"
            id="name"
            label="Parent Folder"
            variant="standard"
            value={foldersStore.editFolderSelected?.name}
          />
        </Grid>
        <Grid item xs={12} mb={2}>
          <TextField
            sx={{ minWidth: 350 }}
            fullWidth
            margin="normal"
            type="text"
            id="name"
            label="Folder Name"
            variant="outlined"
            onChange={(e) => {
              setFolderModel((value) => ({
                ...value,
                name: e.target.value,
              }));
              setErrModel((value) => ({
                ...value,
                name: "",
              }));
              setIsDirty(true);
            }}
            error={errModel.name !== ""}
            helperText={errModel.name}
            value={folderModel.name}
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
