import { Box, Dialog, DialogContent, Tab, Tabs } from "@mui/material";
import { useEffect, useState } from "react";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { LuceneSearchForm } from "./LuceneSearchForm";
import { AiSearchForm } from "./AiSearchForm";
import { ExtendSearchForm } from "./ExtendSearchForm";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
  positionId?: number;
}

export const ComplexSearchFormDialog = ({ isOpen, onClose, positionId }: IProps) => {
  const [open, setOpen] = useState(false);
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog open={open} onClose={onClose} keepMounted PaperProps={{ sx: { minHeight: "60vh", width: "50vw" } }}>
      <BootstrapDialogTitle id="dialog-title" onClose={onClose}>
        CV's Search
      </BootstrapDialogTitle>
      <Box sx={{ borderBottom: 1, borderColor: "divider", px: 3 }}>
        <Tabs value={tabValue} onChange={(_, v) => setTabValue(v)}>
          <Tab label="Search" />
          <Tab label="AI Search" />
          {positionId && <Tab label="Extend Search" />}
        </Tabs>
      </Box>
      <DialogContent sx={{ pt: 1 }}>
        {tabValue === 0 && <LuceneSearchForm onClose={onClose} />}
        {tabValue === 1 && <AiSearchForm onClose={onClose} />}
        {tabValue === 2 && positionId && <ExtendSearchForm positionId={positionId} onClose={onClose} />}
      </DialogContent>
    </Dialog>
  );
};