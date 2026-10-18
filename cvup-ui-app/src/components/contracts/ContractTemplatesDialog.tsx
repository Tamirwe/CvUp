import { useEffect, useRef, useState } from "react";
import { observer } from "mobx-react";
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  IconButton,
  List,
  ListItem,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  MdOutlineUploadFile,
  MdOutlineDelete,
  MdOutlineEdit,
  MdClose,
  MdOutlineWarningAmber,
} from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { IContractTemplate } from "../../models/GeneralModels";
import { AlertConfirmDialogEnum } from "../../models/GeneralEnums";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ContractTemplatesDialog = observer(({ isOpen, onClose }: IProps) => {
  const { customerContractsStore, generalStore } = useStore();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [name, setName] = useState("");
  const [descr, setDescr] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [editingId, setEditingId] = useState(0);
  const [error, setError] = useState("");

  useEffect(() => {
    if (isOpen) {
      customerContractsStore.getContractTemplates();
      resetForm();
    }
  }, [isOpen]); // eslint-disable-line react-hooks/exhaustive-deps

  const resetForm = () => {
    setName("");
    setDescr("");
    setFile(null);
    setEditingId(0);
    setError("");

    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleFile = (selected: File | null) => {
    if (!selected) {
      setFile(null);
      return;
    }

    // The server refuses anything else; saying so here saves the round trip.
    if (!selected.name.toLowerCase().endsWith(".docx")) {
      setError("Only .docx files can be used as a contract template.");
      setFile(null);
      return;
    }

    setError("");
    setFile(selected);

    // The name is the file name, so it keeps the extension.
    if (!name) {
      setName(selected.name);
    }
  };

  const handleSave = async () => {
    if (!name.trim()) {
      setError("A file name is needed.");
      return;
    }

    if (editingId) {
      const template = customerContractsStore.contractTemplatesList.find(
        (x) => x.id === editingId
      );

      if (template) {
        await customerContractsStore.updateContractTemplate({
          ...template,
          name,
          descr,
        });
      }
    } else {
      if (!file) {
        setError("Choose a .docx file to upload.");
        return;
      }

      const res = await customerContractsStore.addContractTemplate(
        name,
        descr,
        file
      );

      if (!res.isSuccess) {
        setError("The template could not be uploaded.");
        return;
      }
    }

    resetForm();
  };

  const handleEdit = (template: IContractTemplate) => {
    setEditingId(template.id);
    setName(template.name);
    setDescr(template.descr ?? "");
    setFile(null);
    setError("");
  };

  const handleDelete = async (template: IContractTemplate) => {
    const isOk = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete template",
      `Delete "${template.name}" and its file?`
    );

    if (isOk) {
      await customerContractsStore.deleteContractTemplate(template.id);

      if (editingId === template.id) {
        resetForm();
      }
    }
  };

  return (
    <Dialog open={isOpen} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle sx={{ fontSize: "1.1rem" }}>Contract templates</DialogTitle>
      <DialogContent dividers>
        <Stack spacing={1.5}>
          <Typography sx={{ fontSize: "0.8rem" }} color="#5d6b77">
            Word documents (.docx) holding the contract text. Placeholders{" "}
            {"{{CustomerName}}"}, {"{{CustomerAddress}}"},{" "}
            {"{{CustomerDescr}}"} and {"{{Date}}"} are filled in when the
            contract is sent.
          </Typography>

          {error && <Alert severity="error">{error}</Alert>}

          <TextField
            label="File name"
            size="small"
            value={name}
            onChange={(e) => setName(e.target.value)}
            helperText={
              editingId
                ? "Renaming also renames the document on the server."
                : "The name the document is stored under. .docx is added if missing."
            }
          />
          <TextField
            label="Description"
            size="small"
            multiline
            minRows={2}
            value={descr}
            onChange={(e) => setDescr(e.target.value)}
          />

          {!editingId && (
            <Box>
              <input
                ref={fileInputRef}
                type="file"
                accept=".docx"
                hidden
                onChange={(e) => handleFile(e.target.files?.[0] ?? null)}
              />
              <Button
                variant="outlined"
                size="small"
                startIcon={<MdOutlineUploadFile />}
                onClick={() => fileInputRef.current?.click()}
              >
                {file ? file.name : "Choose .docx file"}
              </Button>
            </Box>
          )}

          <Stack direction="row" spacing={1}>
            <Button variant="contained" size="small" onClick={handleSave}>
              {editingId ? "Save" : "Upload"}
            </Button>
            {editingId > 0 && (
              <Button size="small" onClick={resetForm}>
                Cancel
              </Button>
            )}
          </Stack>

          <Divider />

          <List dense disablePadding>
            {customerContractsStore.contractTemplatesList.map((template) => (
              <ListItem
                key={template.id}
                disableGutters
                secondaryAction={
                  <Stack direction="row" spacing={0.5}>
                    <IconButton size="small" onClick={() => handleEdit(template)}>
                      <MdOutlineEdit />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => handleDelete(template)}
                    >
                      <MdOutlineDelete />
                    </IconButton>
                  </Stack>
                }
              >
                <Box sx={{ paddingRight: 6 }}>
                  <Stack direction="row" alignItems="center" spacing={0.5}>
                    <Typography sx={{ fontSize: "0.9rem", fontWeight: 600 }}>
                      {template.name}
                    </Typography>
                    {!template.fileExists && (
                      <Tooltip title="The file is missing from the company folder, so this template cannot be sent.">
                        <Box
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            color: "#d84315",
                          }}
                        >
                          <MdOutlineWarningAmber />
                        </Box>
                      </Tooltip>
                    )}
                  </Stack>
                  <Typography sx={{ fontSize: "0.75rem" }} color="#8a97a3">
                    {template.descr}
                  </Typography>
                </Box>
              </ListItem>
            ))}
            {customerContractsStore.contractTemplatesList.length === 0 && (
              <Typography sx={{ fontSize: "0.85rem" }} color="#8a97a3">
                No templates yet. The first upload creates the
                customerContractTemplates folder.
              </Typography>
            )}
          </List>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} startIcon={<MdClose />}>
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
});
