import { useStore } from "../../Hooks/useStore";
import { IconButton, Grid, Stack } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { CiLogout, CiEdit } from "react-icons/ci";
import { SearchCands } from "./SearchCands";
import { SettingsMenu } from "./SettingsMenu";
import { EmailMenu } from "./EmailMenu";
import { EmailTypeEnum } from "../../models/GeneralEnums";
import { useState } from "react";
import { CandidateEmailSender } from "../email/CandidateEmailSender";
import { ContactEmailSender } from "../email/ContactEmailSender";

export const Header = () => {
  const { authStore, candsStore } = useStore();
  const navigate = useNavigate();
  const [emailShow, setEmailShow] = useState<EmailTypeEnum>(EmailTypeEnum.None);

  const handleEmailTypeClick = (emailType: EmailTypeEnum) => {
    setEmailShow(emailType);
  };

  return (
    <div style={{ position: "relative", backgroundColor: "#f3f4f5" }}>
      {emailShow === EmailTypeEnum.Candidate && (
        <CandidateEmailSender
          onClose={() => setEmailShow(EmailTypeEnum.None)}
          open={emailShow === EmailTypeEnum.Candidate}
        />
      )}
      {emailShow === EmailTypeEnum.Contact && (
        <ContactEmailSender
          onClose={() => setEmailShow(EmailTypeEnum.None)}
          open={emailShow === EmailTypeEnum.Contact}
        />
      )}
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
            backgroundColor: "#fff",
            padding: "0 10px",
            height: "100%",
            display: "flex",
            alignItems: "center",
          }}
        >
          <Grid container spacing={0}>
            <Grid item xs={7}>
              <Stack direction="row" alignItems="center" spacing={1}>
                <IconButton
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
                  onClick={() => {
                    if (candsStore.cvReviewDialogOpen) {
                      localStorage.setItem("rteX", "50");
                      localStorage.setItem("rteY", "50");
                    }

                    candsStore.cvReviewDialogOpen =
                      !candsStore.cvReviewDialogOpen;
                  }}
                >
                  <CiEdit />
                </IconButton>
                <EmailMenu onEmailTypeClick={handleEmailTypeClick} />
                <SettingsMenu />
                <SearchCands />
              </Stack>
            </Grid>
            <Grid item xs={5}></Grid>
          </Grid>
        </div>
      </div>
    </div>
  );
};
