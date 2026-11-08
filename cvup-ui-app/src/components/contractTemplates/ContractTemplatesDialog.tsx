import {
  Box,
  Button,
  Dialog,
  DialogContent,
  Divider,
  FormHelperText,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { Viewer, Worker } from "@react-pdf-viewer/core";
import "@react-pdf-viewer/core/lib/styles/index.css";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdOutlineDelete } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { AlertConfirmDialogEnum } from "../../models/GeneralEnums";
import { IContractTemplate } from "../../models/GeneralModels";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ContractTemplatesDialog = observer(({ isOpen, onClose }: IProps) => {
  const { contractTemplatesStore, generalStore } = useStore();
  const [file, setFile] = useState<File | null>(null);
  const [name, setName] = useState("");
  const [descr, setDescr] = useState("");
  const [submitError, setSubmitError] = useState("");
  const [selectedId, setSelectedId] = useState(0);
  const [previewUrl, setPreviewUrl] = useState("");

  useEffect(() => {
    contractTemplatesStore.getContractTemplates();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // Releases the previous document when another one is shown or the dialog closes
  useEffect(() => {
    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  }, [previewUrl]);

  const showTemplate = async (template: IContractTemplate) => {
    setSelectedId(template.id);

    const res = await contractTemplatesStore.getContractTemplateFile(template.id);

    if (res.isSuccess && res.data) {
      setPreviewUrl(URL.createObjectURL(res.data));
    } else {
      setPreviewUrl("");
      generalStore.alertSnackbar("error", "Failed to load the document");
    }
  };

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
      setFile(null);
      setName("");
      setDescr("");
      setSubmitError("");
      generalStore.alertSnackbar("success", "Contract template uploaded");
      await contractTemplatesStore.getContractTemplates();
      await showTemplate(res.data);
    } else {
      generalStore.alertSnackbar("error", "Failed to upload the document");
    }
  };

  const deleteTemplate = async (template: IContractTemplate) => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete contract template",
      `Are you sure you want to delete "${template.name}"?`
    );

    if (!isDelete) {
      return;
    }

    const res = await contractTemplatesStore.deleteContractTemplate(template.id);

    if (res.isSuccess) {
      if (template.id === selectedId) {
        setSelectedId(0);
        setPreviewUrl("");
      }

      generalStore.alertSnackbar("success", "Contract template deleted");
      await contractTemplatesStore.getContractTemplates();
    } else {
      generalStore.alertSnackbar("error", "Failed to delete the template");
    }
  };

  return (
    <Dialog open={isOpen} onClose={onClose} fullWidth maxWidth={"lg"}>
      <BootstrapDialogTitle id="contract-templates-title" onClose={onClose}>
        Contract Templates
      </BootstrapDialogTitle>
      <DialogContent>
        <Stack
          direction={{ xs: "column", md: "row" }}
          alignItems={{ xs: "stretch", md: "flex-start" }}
          gap={2}
          sx={{ pt: 1 }}
        >
          <Box sx={{ flexShrink: 0, width: { xs: "100%", md: 220 } }}>
            <Button
              fullWidth
              variant="outlined"
              component="label"
              sx={{ height: 56 }}
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
          </Box>
          <TextField
            fullWidth
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
            id="contractDescr"
            label="Contract description"
            variant="outlined"
            inputProps={{ maxLength: 1000 }}
            value={descr}
            onChange={(e) => setDescr(e.target.value)}
          />
          <Button
            variant="contained"
            color="secondary"
            onClick={uploadTemplate}
            sx={{ height: 56, flexShrink: 0 }}
          >
            Upload
          </Button>
        </Stack>
        <FormHelperText error>{submitError}</FormHelperText>

        <Divider sx={{ my: 1 }} />

        <Stack direction={{ xs: "column", md: "row" }} gap={2}>
          <List
            sx={{
              width: { xs: "100%", md: 280 },
              flexShrink: 0,
              overflow: "auto",
              maxHeight: { xs: 200, md: "65vh" },
            }}
          >
            {contractTemplatesStore.contractTemplatesList.map((item) => (
              <ListItem
                key={item.id}
                disablePadding
                divider
                secondaryAction={
                  <IconButton
                    edge="end"
                    aria-label={`Delete ${item.name}`}
                    title="Delete"
                    onClick={() => deleteTemplate(item)}
                  >
                    <MdOutlineDelete />
                  </IconButton>
                }
              >
                <ListItemButton
                  selected={item.id === selectedId}
                  onClick={() => showTemplate(item)}
                >
                  <ListItemText primary={item.name} secondary={item.descr} />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
          <Box sx={{ flexGrow: 1, minWidth: 0, height: "65vh", overflow: "auto" }}>
            {previewUrl ? (
              <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
                <Viewer key={previewUrl} fileUrl={previewUrl} />
              </Worker>
            ) : (
              <Typography color="text.secondary" sx={{ pt: 2, textAlign: "center" }}>
                Select a template to view it
              </Typography>
            )}
          </Box>
        </Stack>
      </DialogContent>
    </Dialog>
  );
});
