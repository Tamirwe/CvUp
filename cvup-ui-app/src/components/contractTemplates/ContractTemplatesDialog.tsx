import {
  Box,
  Button,
  Dialog,
  DialogContent,
  Divider,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Stack,
  Typography,
} from "@mui/material";
import { Viewer, Worker } from "@react-pdf-viewer/core";
import "@react-pdf-viewer/core/lib/styles/index.css";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { GoPlus } from "react-icons/go";
import { MdOutlineDelete } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { AlertConfirmDialogEnum } from "../../models/GeneralEnums";
import { IContractTemplate } from "../../models/GeneralModels";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { ContractSendForm } from "./ContractSendForm";
import { ContractUploadDialog } from "./ContractUploadDialog";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const ContractTemplatesDialog = observer(({ isOpen, onClose }: IProps) => {
  const { contractTemplatesStore, generalStore } = useStore();
  const [isUploadOpen, setIsUploadOpen] = useState(false);
  const [selectedId, setSelectedId] = useState(0);
  const [previewUrl, setPreviewUrl] = useState("");
  // Changing the key mounts a new, empty send form
  const [sendFormKey, setSendFormKey] = useState(0);

  const selectedTemplate = contractTemplatesStore.contractTemplatesList.find(
    (x) => x.id === selectedId
  );

  useEffect(() => {
    contractTemplatesStore.getContractTemplates();
    contractTemplatesStore.getContractEmailTemplates();
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

  const onTemplateUploaded = async (template: IContractTemplate) => {
    setIsUploadOpen(false);
    await contractTemplatesStore.getContractTemplates();
    await showTemplate(template);
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
        Send Contract to Customer
      </BootstrapDialogTitle>
      <DialogContent>
        {isUploadOpen && (
          <ContractUploadDialog
            isOpen={isUploadOpen}
            onClose={() => setIsUploadOpen(false)}
            onUploaded={onTemplateUploaded}
          />
        )}

        <ContractSendForm
          key={sendFormKey}
          template={selectedTemplate}
          onSent={() => setSendFormKey((key) => key + 1)}
        />

        <Divider sx={{ my: 2 }} />

        <Stack direction={{ xs: "column", md: "row" }} gap={2} sx={{ pt: 1 }}>
          <Box
            sx={{
              width: { xs: "100%", md: 280 },
              flexShrink: 0,
              height: { md: "65vh" },
              display: "flex",
              flexDirection: "column",
            }}
          >
            <List
              sx={{
                flexGrow: 1,
                overflow: "auto",
                maxHeight: { xs: 200, md: "none" },
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
            <Button
              startIcon={<GoPlus />}
              onClick={() => setIsUploadOpen(true)}
              sx={{ alignSelf: "flex-start", flexShrink: 0, mt: 1 }}
            >
              Upload contract
            </Button>
          </Box>
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
