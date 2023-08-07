import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { AlertConfirmDialogEnum } from "../models/GeneralEnums";

export const AlertConfirmDialog = observer(() => {
  const { generalStore } = useStore();

  const handleOk = () => {
    generalStore.confirmResponse(true);
    generalStore.alertConfirmDialogOpen = false;
  };

  const handleCancel = () => {
    generalStore.confirmResponse(false);
    generalStore.alertConfirmDialogOpen = false;
  };

  return (
    <div>
      <Dialog
        open={generalStore.alertConfirmDialogOpen}
        onClose={handleCancel}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {generalStore.alertConfirmDialogTitle}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            <div>{generalStore.alertConfirmDialogMessage}</div>
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleOk}>OK</Button>
          {generalStore.alertConfirmDialogType ===
            AlertConfirmDialogEnum.Confirm && (
            <Button onClick={handleCancel} autoFocus>
              Cancel
            </Button>
          )}
        </DialogActions>
      </Dialog>
    </div>
  );
});
