import { useStore } from "../../Hooks/useStore";
import { IconButton, Grid, Stack } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { CiLogout, CiEdit } from "react-icons/ci";
import { SearchCands } from "./SearchCands";
import { SettingsMenu } from "./SettingsMenu";
import { EmailSender } from "./EmailSender";

export const Header = () => {
  const { authStore, candsStore } = useStore();
  const navigate = useNavigate();

  return (
    <div style={{ position: "relative", backgroundColor: "#f3f4f5" }}>
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
                    candsStore.cvReviewDialogOpen = true;
                  }}
                >
                  <CiEdit />
                </IconButton>
                <EmailSender />
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
