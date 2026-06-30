import { Box, Dialog, DialogContent, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { PositionForm } from "./PositionForm";
import { AnalyzedPositionData } from "./AnalyzedPositionData";
import { PositionAiRewrite } from "./PositionAiRewrite";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const PositionFormDialog = ({ isOpen, onClose }: IProps) => {
  const { positionsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [formTitle, setFormTitle] = useState("Add Position");
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    if (positionsStore.editPosition) {
      setFormTitle("Edit Position");
    }
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth={"lg"} PaperProps={{ sx: { minHeight: "60vh" } }}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        {formTitle}
      </BootstrapDialogTitle>
      <Box sx={{ borderBottom: 1, borderColor: "divider", px: 3 }}>
        <Tabs value={tabValue} onChange={(_, v) => setTabValue(v)}>
          <Tab label="Details" />
          <Tab label="Analyzed Data" />
          <Tab label="AI Rewrite" />
        </Tabs>
      </Box>
      <DialogContent sx={{ pt: 1 }}>
        {tabValue === 0 && <PositionForm onClose={onClose} />}
        {tabValue === 1 && <AnalyzedPositionData />}
        {tabValue === 2 && <PositionAiRewrite />}
      </DialogContent>
    </Dialog>
  );
};
