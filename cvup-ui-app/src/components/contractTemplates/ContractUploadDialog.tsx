import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  FormHelperText,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IContractTemplate } from "../../models/GeneralModels";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
  onUploaded: (template: IContractTemplate) => void;
}

export const ContractUploadDialog = ({ isOpen, onClose, onUploaded }: IProps) => {
  const { contractTemplatesStore, generalStore } = useStore();
  const [file, setFile] = useState<File | null>(null);
  const [name, setName] = useState("");
  const [descr, setDescr] = useState("");
  const [submitError, setSubmitError] = useState("");

  const uploadTemplate = async () => {
    if (!file) {
      setSubmitError("Select a document to upload");
      return;
    }

    if (!name.trim()) {
      setSubmitError("Enter the contract name");
      return;
    }

    const res = await contractTemplatesStore.addContractTemplate(
      name.trim(),
      descr.trim(),
      file
    );

    if (res.isSuccess && res.data) {
      generalStore.alertSnackbar("success", "Contract template uploaded");
      onUploaded(res.data);
    } else {
      generalStore.alertSnackbar("error", "Failed to upload the document");
    }
  };

  return (
    <Dialog open={isOpen} onClose={onClose} fullWidth maxWidth={"sm"}>
      <BootstrapDialogTitle id="contract-upload-title" onClose={onClose}>
        Upload Contract
      </BootstrapDialogTitle>
      <DialogContent>
        <Button
          fullWidth
          variant="outlined"
          component="label"
          sx={{ height: 56, mt: 1 }}
        >
          Choose document
          <input
            hidden
            type="file"
            accept=".pdf,.doc,.docx"
            onChange={(e) => {
              setFile(e.target.files?.[0] || null);
              setSubmitError("");
              e.target.value = "";
            }}
          />
        </Button>
        <Typography variant="caption" color="text.secondary" noWrap component="div">
          {file?.name || "pdf, doc or docx"}
        </Typography>
        <TextField
          fullWidth
          margin="normal"
          id="contractName"
          label="Contract name"
          variant="outlined"
          inputProps={{ maxLength: 300 }}
          value={name}
          onChange={(e) => {
            setName(e.target.value);
            setSubmitError("");
          }}
        />
        <TextField
          sx={{ direction: "rtl" }}
          fullWidth
          multiline
          rows={3}
          margin="normal"
          id="contractDescr"
          label="Contract description"
          variant="outlined"
          inputProps={{ maxLength: 1000 }}
          value={descr}
          onChange={(e) => setDescr(e.target.value)}
        />
        <FormHelperText error>{submitError}</FormHelperText>
      </DialogContent>
      <DialogActions>
        <Button color="secondary" onClick={onClose}>
          Cancel
        </Button>
        <Button variant="contained" color="secondary" onClick={uploadTemplate}>
          Upload
        </Button>
      </DialogActions>
    </Dialog>
  );
};
