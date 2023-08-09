import {
  Alert,
  Backdrop,
  Box,
  CircularProgress,
  Snackbar,
} from "@mui/material";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  CrudTypesEnum,
  EmailTypeEnum,
} from "../models/GeneralEnums";
import { CandidateEmailSender } from "../components/email/CandidateEmailSender";
import { ContactEmailSender } from "../components/email/ContactEmailSender";
import { ContactsFormDialog } from "../components/contacts/ContactsFormDialog";
import { FolderFormDialog } from "../components/folders/FolderFormDialog";
import { useEffect, useState } from "react";
import { AlertConfirmDialog } from "./AlertConfirmDialog";
import { UsersFormDialog } from "../components/users/UsersFormDialog";
import { CustomersListDialog } from "../components/customers/CustomersListDialog";
import { UsersListDialog } from "../components/users/UsersListDialog";
import { PositionFormDialog } from "../components/positions/PositionFormDialog";
import { BrowserView, MobileView } from "react-device-detect";
import { BrowserAuthLayout } from "./BrowserAuthLayout";
import { MobileAuthLayout } from "./MobileAuthLayout";
import { ReviewCandDialog } from "../components/cands/ReviewCandDialog";
import { EmailTemplateFormDialog } from "../components/email/EmailTemplateFormDialog";
import { CandFormDialog } from "../components/cands/CandFormDialog";

export const LayoutAuthWrapper = observer(() => {
  const { generalStore } = useStore();
  const [pause, setPause] = useState(false);

  useEffect(() => {
    const interval = setInterval(async () => {
      if (!pause) {
        const res = await generalStore.getIsAuthorized();

        if (!res.isSuccess) {
          setPause(true);
          const isOk = await generalStore.alertConfirmDialog(
            AlertConfirmDialogEnum.Confirm,
            "Your session expired",
            "Please login again"
          );

          setPause(false);

          if (isOk) {
            document.location.href = "/";
          }
        }
      }
    }, 60000);

    return () => clearInterval(interval);
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <Box sx={{ flexGrow: 1 }}>
      {generalStore.showEmailDialog === EmailTypeEnum.Candidate && (
        <CandidateEmailSender
          onClose={() => (generalStore.showEmailDialog = EmailTypeEnum.None)}
          open={generalStore.showEmailDialog === EmailTypeEnum.Candidate}
        />
      )}
      {generalStore.showEmailDialog === EmailTypeEnum.Contact && (
        <ContactEmailSender
          onClose={() => (generalStore.showEmailDialog = EmailTypeEnum.None)}
          open={generalStore.showEmailDialog === EmailTypeEnum.Contact}
        />
      )}
      {(generalStore.openModeFolderFormDialog as CrudTypesEnum) !==
        CrudTypesEnum.None && (
        <FolderFormDialog
          isOpen={
            (generalStore.openModeFolderFormDialog as CrudTypesEnum) !==
            CrudTypesEnum.None
          }
          onClose={() =>
            (generalStore.openModeFolderFormDialog = CrudTypesEnum.None)
          }
        />
      )}
      {generalStore.showPositionFormDialog && (
        <PositionFormDialog
          isOpen={generalStore.showPositionFormDialog}
          onClose={() => (generalStore.showPositionFormDialog = false)}
        />
      )}
      {generalStore.showContactFormDialog && (
        <ContactsFormDialog
          isOpen={generalStore.showContactFormDialog}
          onClose={() => (generalStore.showContactFormDialog = false)}
        />
      )}
      {generalStore.showCustomersListDialog && (
        <CustomersListDialog
          isOpen={generalStore.showCustomersListDialog}
          onClose={() => (generalStore.showCustomersListDialog = false)}
        />
      )}
      {generalStore.showUserFormDialog && (
        <UsersFormDialog
          isOpen={generalStore.showUserFormDialog}
          onClose={() => (generalStore.showUserFormDialog = false)}
        />
      )}
      {generalStore.showUserListDialog && (
        <UsersListDialog
          isOpen={generalStore.showUserListDialog}
          onClose={() => (generalStore.showUserListDialog = false)}
        />
      )}
      {generalStore.showReviewCandDialog && (
        <ReviewCandDialog
          isOpen={generalStore.showReviewCandDialog}
          onClose={() => (generalStore.showReviewCandDialog = false)}
        />
      )}
      {generalStore.showEmailTemplatesDialog && (
        <EmailTemplateFormDialog
          isOpen={generalStore.showEmailTemplatesDialog}
          onClose={() => (generalStore.showEmailTemplatesDialog = false)}
        />
      )}
      {generalStore.showCandFormDialog && (
        <CandFormDialog
          isOpen={generalStore.showCandFormDialog}
          onClose={() => (generalStore.showCandFormDialog = false)}
        />
      )}
      {generalStore.alertConfirmDialogOpen && <AlertConfirmDialog />}

      <Snackbar
        anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
        autoHideDuration={3500}
        open={generalStore.alertSnackbarOpen}
        onClose={() => (generalStore.alertSnackbarOpen = false)}
      >
        <Alert
          variant="filled"
          onClose={() => (generalStore.alertSnackbarOpen = false)}
          severity={generalStore.alertSnackbarType}
          sx={{ width: "100%" }}
        >
          {generalStore.alertSnackbarMessage}
        </Alert>
      </Snackbar>
      <Backdrop
        sx={{ color: "#a0dbff", zIndex: 99999, backgroundColor: "#0000000f" }}
        open={generalStore.backdrop}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
      <BrowserView>
        <BrowserAuthLayout />
      </BrowserView>
      <MobileView>
        <MobileAuthLayout />
      </MobileView>
    </Box>
  );
});
