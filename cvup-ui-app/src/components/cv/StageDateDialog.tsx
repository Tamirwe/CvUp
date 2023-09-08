import { Button, Dialog, DialogContent, Grid, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { DateCalendar } from "@mui/x-date-pickers/DateCalendar";
import { format } from "date-fns";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
  onRemoveStage: () => void;
  onNewStageDate: (newDate: Date) => void;
}

export const StageDateDialog = ({
  isOpen,
  onClose,
  onRemoveStage,
  onNewStageDate,
}: IProps) => {
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  return (
    <Dialog open={open} fullWidth maxWidth={"xs"} sx={{}}>
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose()}>
        Stage Date
      </BootstrapDialogTitle>
      <DialogContent>
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <DateCalendar
            sx={{ border: "1px solid #e1eaff" }}
            onChange={(newValue) =>
              onNewStageDate(new Date(newValue as string))
            }
          />
        </LocalizationProvider>
        <Grid container pt={4}>
          <Grid item xs={12}>
            <Stack direction="row" alignItems="center" gap={1}>
              <Button fullWidth color="secondary" onClick={onRemoveStage}>
                Remove Stage
              </Button>

              <Button fullWidth color="secondary" onClick={() => onClose()}>
                Cancel
              </Button>
            </Stack>
          </Grid>
        </Grid>
      </DialogContent>
    </Dialog>
  );
};
