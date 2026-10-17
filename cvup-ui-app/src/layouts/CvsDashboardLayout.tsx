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
import { useEffect } from "react";
import { AlertConfirmDialog } from "./AlertConfirmDialog";
import { UsersFormDialog } from "../components/users/UsersFormDialog";
import { CustomersListDialog } from "../components/customers/CustomersListDialog";
import { UsersListDialog } from "../components/users/UsersListDialog";
import { PositionFormDialog } from "../components/positions/PositionFormDialog";
import { BrowserView, MobileView, isMobile } from "react-device-detect";
import { MobileAuthLayout } from "./MobileAuthLayout";
import { ReviewCandDialog } from "../components/cands/ReviewCandDialog";
import { EmailTemplateFormDialog } from "../components/email/EmailTemplateFormDialog";
import { CandFormDialog } from "../components/cands/CandFormDialog";
import { BrowserAuthLayout } from "./BrowserAuthLayout";
import { CustomerReviewCandDialog } from "../components/cands/CustomerReviewCandDialog";
import { SearchesListDialog } from "../components/searches/SearchesListDialog";
import { EditSearchesListDialog } from "../components/searches/EditSearchesListDialog";
import { StageDateDialog } from "../components/cv/StageDateDialog";
import { InterviewFullDialog } from "../components/cands/InterviewFullDialog";
import { RestoreReviewDialog } from "../components/cands/RestoreReviewDialog";

const AUTH_CHECK_INTERVAL_MS = 60000;
const MAX_NOT_AUTH_COUNT = 5;

export const LayoutAuthWrapper = observer(() => {
  const { generalStore } = useStore();

  useEffect(() => {
    let countNotAuth = 0;
    let isChecking = false;

    const checkAuthorized = async () => {
      // The tab can trigger several checks at once when it wakes up, and the whole
      // dialog wait happens inside this flag, so the expired dialog cannot stack.
      if (isChecking) {
        return;
      }

      // A dead network is not an expired session. Waking from sleep routinely fails
      // the first pings while wifi reassociates.
      if (!navigator.onLine) {
        return;
      }

      isChecking = true;

      try {
        const res = await generalStore.getIsAuthorized();

        if (res.isSuccess) {
          countNotAuth = 0;
          return;
        }

        // Only an actual rejection counts towards expiry - a timeout or a 502 means
        // the server is unhappy, not that the user has been logged out.
        if (res.status !== 401 && res.status !== 403) {
          return;
        }

        countNotAuth++;

        if (countNotAuth >= MAX_NOT_AUTH_COUNT) {
          const isOk = await generalStore.alertConfirmDialog(
            AlertConfirmDialogEnum.Confirm,
            "Your session expired",
            "Please login again"
          );

          countNotAuth = 0;

          if (isOk) {
            document.location.href = "/";
          }
        }
      } finally {
        isChecking = false;
      }
    };

    const interval = setInterval(checkAuthorized, AUTH_CHECK_INTERVAL_MS);

    // Chrome throttles and then freezes timers in background tabs, so after the tab
    // has been idle this interval may not have run for hours. Check as soon as the
    // tab is usable again instead of waiting five more minutes to notice.
    const checkOnWakeUp = () => {
      if (document.visibilityState === "visible") {
        checkAuthorized();
      }
    };

    document.addEventListener("visibilitychange", checkOnWakeUp);
    window.addEventListener("focus", checkOnWakeUp);
    window.addEventListener("online", checkOnWakeUp);

    return () => {
      clearInterval(interval);
      document.removeEventListener("visibilitychange", checkOnWakeUp);
      window.removeEventListener("focus", checkOnWakeUp);
      window.removeEventListener("online", checkOnWakeUp);
    };
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
      {generalStore.showReviewCandDialog && isMobile && (
        <ReviewCandDialog
          isOpen={generalStore.showReviewCandDialog}
          onClose={() => (generalStore.showReviewCandDialog = false)}
        />
      )}
      {generalStore.showInterviewFullDialog && (
        <InterviewFullDialog
          isOpen={generalStore.showInterviewFullDialog}
          onClose={() => (generalStore.showInterviewFullDialog = false)}
        />
      )}
      {generalStore.showCustomerReviewCandDialog && (
        <CustomerReviewCandDialog
          isOpen={generalStore.showCustomerReviewCandDialog}
          onClose={() => (generalStore.showCustomerReviewCandDialog = false)}
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
      {generalStore.showSearchesListDialog && (
        <SearchesListDialog
          isOpen={generalStore.showSearchesListDialog}
          onClose={() => (generalStore.showSearchesListDialog = false)}
        />
      )}
      {generalStore.showRestoreReviewDialog && (
        <RestoreReviewDialog
          isOpen={generalStore.showRestoreReviewDialog}
          onClose={() => (generalStore.showRestoreReviewDialog = false)}
        />
      )}
      {generalStore.showEditSearchesListDialog && (
        <EditSearchesListDialog
          isOpen={generalStore.showEditSearchesListDialog}
          onClose={() => (generalStore.showEditSearchesListDialog = false)}
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
