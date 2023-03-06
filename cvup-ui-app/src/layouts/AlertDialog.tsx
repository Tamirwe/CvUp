import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";

export const AlertDialog = observer(() => {
  const { generalStore } = useStore();

  const handleOk = () => {
    generalStore.confirmResponse(true);
    generalStore.confirmDialogOpen = false;
  };

  const handleCancel = () => {
    generalStore.confirmResponse(false);
    generalStore.confirmDialogOpen = false;
  };

  return (
    <div>
      <Dialog
        open={generalStore.confirmDialogOpen}
        onClose={handleCancel}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {generalStore.confirmDialogTitle}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            {generalStore.confirmDialogMessage}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleOk}>OK</Button>
          <Button onClick={handleCancel} autoFocus>
            Cancel
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
});
