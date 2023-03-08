import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IFolder } from "../../models/GeneralModels";
import { textFieldValidte } from "../../utils/Validation";
import { FolderSettingsMenu } from "./FolderSettingsMenu";

interface IProps {
  onAddChild: () => void;
  onSaved: () => void;
  onCancel: () => void;
}

export const FormUpdateDelete = ({ onAddChild, onSaved, onCancel }: IProps) => {
  const { foldersStore, generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [folderModel, setFolderModel] = useState<IFolder>({
    id: 0,
    name: "",
    parentId: 0,
  });
  const [errModel, setErrModel] = useState({
    name: "",
  });
  // const [formValErrorTxt, setFormValErrorTxt] = useState({
  //   name: "",
  // });

  useEffect(() => {
    if (foldersStore.editFolderSelected) {
      setFolderModel({ ...foldersStore.editFolderSelected });
    }
  }, [foldersStore.editFolderSelected]);

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
    const response = await foldersStore.updateFolder(folderModel);
    if (response.isSuccess) {
      onSaved();
    } else {
      return setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  const deleteRecord = async () => {
    const isDelete = await generalStore.confirmDialog(
      "Delete Folder",
      "Are you sure you want to delete this folder?"
    );

    if (isDelete) {
      const response = await foldersStore.deleteFolder(folderModel.id);

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }

    // const response = await generalStore.deleteHrCompany(formModel.id);
    // if (response.isSuccess) {
    //   onSaved();
    // } else {
    //   return setSubmitError("An Error Occurred Please Try Again Later.");
    // }
  };

  const handleMenuSelected = async (menuItem: string) => {
    switch (menuItem) {
      case "delete":
        await deleteRecord();
        break;
      case "addChild":
        onAddChild();
        break;
      default:
        break;
    }
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

        <div
          style={{
            paddingTop: 30,
            display: "flex",
            justifyContent: "space-between",
            width: "inherit",
          }}
        >
          <div>
            <FolderSettingsMenu onMenuSelected={handleMenuSelected} />
          </div>
          <div style={{ display: "flex", gap: 8 }}>
            <Button
              disabled={!isDirty}
              fullWidth
              variant="text"
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
            <Button
              fullWidth
              variant="text"
              color="secondary"
              onClick={() => onCancel()}
            >
              Cancel
            </Button>
          </div>
        </div>
      </Grid>
    </form>
  );
};
