import { useStore } from "../../Hooks/useStore";
import { Grid, IconButton, Stack } from "@mui/material";
import { SettingsMenu } from "./SettingsMenu";
import { useEffect } from "react";
import { BsList } from "react-icons/bs";
import { isMobile } from "react-device-detect";
import {
  MdOutlineAttachEmail,
  MdOutlineContactMail,
  MdOutlineEdit,
  MdOutlineMarkEmailRead,
  MdOutlineMarkEmailUnread,
} from "react-icons/md";
import { EmailTypeEnum } from "../../models/GeneralEnums";
import { CiEdit, CiMail } from "react-icons/ci";

export const Header = () => {
  const {
    candsStore,
    generalStore,
    positionsStore,
    customersContactsStore,
    authStore,
  } = useStore();
  // const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      await Promise.all([
        candsStore.getCompanyStagesTypes(),
        positionsStore.getPositionsList(),
        candsStore.getEmailTemplates(),
        customersContactsStore.getCustomersList(),
        authStore.getUser(),
      ]);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <div
      style={{
        position: "relative",
        backgroundColor: "#f3f4f5",
        height: "3rem",
      }}
    >
      <div
        style={{
          position: "absolute",
          height: "3rem",
          width: "100%",
          zIndex: 1290,
          backgroundColor: "#fff",
        }}
      >
        <div
          style={{
            backgroundColor: "#eaf5ff",
            margin: "5px 10px",
            height: "100%",
            display: "flex",
            alignItems: "center",
            borderRadius: "25px",
          }}
        >
          <Grid container spacing={0}>
            <Grid item xs={7} pl={1}>
              <Stack direction="row" alignItems="center" spacing={1}>
                {/* <IconButton
                  size="medium"
                  onClick={(e) => {
                    authStore.logout();
                    navigate("/login");
                  }}
                >
                  <CiLogout />
                </IconButton>
                <IconButton
                  size="medium"
                  onClick={() =>
                    (generalStore.showEmailDialog = EmailTypeEnum.Candidate)
                  }
                >
                  <MdOutlineContactMail />
                </IconButton>
                <IconButton
                  size="medium"
                  onClick={() =>
                    (generalStore.showEmailDialog = EmailTypeEnum.Contact)
                  }
                >
                  <MdOutlineAttachEmail />
                </IconButton>
                <IconButton
                  size="medium"
                  onClick={() => {
                    if (generalStore.cvReviewDialogOpen) {
                      localStorage.setItem("rteX", "50");
                      localStorage.setItem("rteY", "50");
                    }

                    generalStore.cvReviewDialogOpen =
                      !generalStore.cvReviewDialogOpen;
                  }}
                >
                  <CiEdit />
                </IconButton> */}
                {isMobile && (
                  <IconButton
                    size="medium"
                    onClick={() => (generalStore.leftDrawerOpen = true)}
                  >
                    <BsList />
                  </IconButton>
                )}

                <SettingsMenu />
                <IconButton
                  title="Email to candidate"
                  sx={{ fontSize: "1.56rem" }}
                  size="medium"
                  onClick={() =>
                    (generalStore.showEmailDialog = EmailTypeEnum.Candidate)
                  }
                >
                  <MdOutlineMarkEmailRead />
                </IconButton>
                <IconButton
                  title="Review"
                  sx={{ fontSize: "1.54rem", paddingTop: "0.4rem" }}
                  size="small"
                  onClick={() => {
                    if (generalStore.cvReviewDialogOpen) {
                      localStorage.setItem("rteX", "50");
                      localStorage.setItem("rteY", "50");
                    }

                    generalStore.showReviewCandDialog =
                      !generalStore.showReviewCandDialog;
                  }}
                >
                  <CiEdit />
                </IconButton>
                <IconButton
                  title="Email to customer"
                  sx={{ fontSize: "1.54rem" }}
                  size="small"
                  onClick={() =>
                    (generalStore.showEmailDialog = EmailTypeEnum.Contact)
                  }
                >
                  <MdOutlineMarkEmailUnread />
                </IconButton>
                {/* <SearchCands /> */}
              </Stack>
            </Grid>
            <Grid item xs={5} sx={{ textAlign: "right" }} pr={1}>
              {isMobile && (
                <IconButton
                  size="medium"
                  onClick={() => (generalStore.rightDrawerOpen = true)}
                >
                  <BsList />
                </IconButton>
              )}
            </Grid>
          </Grid>
        </div>
      </div>
    </div>
  );
};
