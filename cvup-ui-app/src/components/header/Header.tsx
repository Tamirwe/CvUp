import { useStore } from "../../Hooks/useStore";
import { Grid, IconButton, Link, Stack } from "@mui/material";
import { SettingsMenu } from "./SettingsMenu";
import { useEffect } from "react";
import { BsList } from "react-icons/bs";
import { isMobile } from "react-device-detect";
import {
  MdOutlineAttachEmail,
  MdOutlineContactMail,
  MdOutlineDelete,
  MdOutlineEdit,
  MdOutlineEditNotifications,
  MdOutlineFileDownload,
  MdOutlineMarkEmailRead,
  MdOutlineMarkEmailUnread,
} from "react-icons/md";
import {
  AlertConfirmDialogEnum,
  EmailTypeEnum,
} from "../../models/GeneralEnums";
import { CiEdit, CiMail } from "react-icons/ci";
import { observer } from "mobx-react";
import { useLocation } from "react-router-dom";

export const Header = observer(() => {
  const location = useLocation();

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
        candsStore.getCandPosStages(),
        positionsStore.getPositionsList(),
        positionsStore.getPositionsTypesList(),
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
            //backgroundColor: "rgb(249 252 255)",
            //border: "1px solid #f3f3f3",
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
                    onClick={() => {
                      generalStore.rightDrawerOpen = false;
                      generalStore.leftDrawerOpen =
                        !generalStore.leftDrawerOpen;
                    }}
                  >
                    <BsList />
                  </IconButton>
                )}

                <SettingsMenu />
                {candsStore.candDisplay && location.pathname === "/cv" && (
                  <>
                    {/* <IconButton
                      title="Email to candidate"
                      sx={{ fontSize: "1.56rem" }}
                      size="medium"
                      onClick={() =>
                        (generalStore.showEmailDialog = EmailTypeEnum.Candidate)
                      }
                    >
                      <MdOutlineMarkEmailRead />
                    </IconButton> */}
                    <IconButton
                      title="Review"
                      sx={{ fontSize: "1.54rem", paddingTop: "0.4rem" }}
                      size="small"
                      onClick={() => {
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
                    {/*
                    <Link
                      href={`${candsStore.downloadUrl}DD/GetFileStream?id=${candsStore.candDisplay.keyId}`}
                      download
                    >
                      <IconButton
                        title="Download original file"
                        sx={{ fontSize: "1.54rem" }}
                        size="small"
                      >
                        <MdOutlineFileDownload />
                      </IconButton>
                    </Link> */}

                    <IconButton
                      title="Download original file"
                      sx={{ fontSize: "1.54rem" }}
                      size="small"
                      onClick={async () => {
                        const keyId = candsStore.candDisplay?.keyId;

                        const data = await candsStore.getfile(keyId);
                        if (!(data instanceof Blob)) return;
                        const downloadedFile = new Blob([data!], {
                          type: data.type,
                        });

                        const a = document.createElement("a");
                        a.setAttribute("style", "display:none;");
                        document.body.appendChild(a);
                        switch (data.type) {
                          case "application/pdf":
                            a.download = `${keyId}.pdf`;
                            break;
                          case "application/msword":
                            a.download = `${keyId}.doc`;
                            break;
                          case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                            a.download = `${keyId}.docx`;
                            break;
                          default:
                            break;
                        }
                        a.href = URL.createObjectURL(downloadedFile);
                        a.target = "_blank";
                        a.click();
                        document.body.removeChild(a);
                      }}
                    >
                      <MdOutlineFileDownload />
                    </IconButton>
                    {candsStore.candDisplay &&
                      positionsStore.candDisplayPosition && (
                        <IconButton
                          title="Customer review"
                          sx={{ fontSize: "1.54rem", paddingTop: "0.4rem" }}
                          size="small"
                          onClick={() => {
                            generalStore.showCustomerReviewCandDialog =
                              !generalStore.showCustomerReviewCandDialog;
                          }}
                        >
                          <MdOutlineEditNotifications />
                        </IconButton>
                      )}
                  </>
                )}
              </Stack>
            </Grid>
            <Grid item xs={5} sx={{ textAlign: "right" }} pr={1}>
              {isMobile && (
                <IconButton
                  size="medium"
                  onClick={() => {
                    generalStore.leftDrawerOpen = false;
                    generalStore.rightDrawerOpen =
                      !generalStore.rightDrawerOpen;
                  }}
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
});
