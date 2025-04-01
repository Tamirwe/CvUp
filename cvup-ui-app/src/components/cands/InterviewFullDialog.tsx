import { Button, Dialog, DialogContent, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { ReviewCandForm } from "./ReviewCandForm";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { PdfViewer } from "../pdfViewer/PdfViewer";
import { observer } from "mobx-react";
import { useStore } from "../../Hooks/useStore";
import { CandsPosStagesList } from "./CandsPosStagesList";
import { CandDupCvsList } from "./CandDupCvsList";
import { CandsSourceEnum } from "../../models/GeneralEnums";
import { PersonalDetails } from "./PersonalDetails";

interface IProps {
  isOpen: boolean;
  onClose: (isSaved: boolean) => void;
}

export const InterviewFullDialog = observer(({ isOpen, onClose }: IProps) => {
  const { candsStore, authStore, generalStore, positionsStore } = useStore();
  const [open, setOpen] = useState(false);
  const [isShowHistory, setIsShowHistory] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleShowHistory = () => {
    setIsShowHistory(!isShowHistory);
  };

  return (
    <Dialog
      open={open}
      // onClose={() => onClose(false)}
      fullScreen
      maxWidth={"md"}
      sx={{}}
    >
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose(false)}>
        Candidate Interview
      </BootstrapDialogTitle>
      <DialogContent sx={{ overflow: "hidden", paddingBottom: "15px" }}>
        <Grid container style={{ height: "100%" }}>
          <Grid item md={6}>
            <div style={{ overflowY: "auto", height: "90vh" }}>
              <PdfViewer />
            </div>
          </Grid>
          <Grid item md={6} sx={{ padding: "0 10px" }}>
            <div
              style={{
                width: "100%",
                height: "90vh",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <div style={{ width: "100%", display: "flex" }}>
                <div>
                  <Button
                    color="secondary"
                    onClick={handleShowHistory}
                    style={{ fontSize: "0.7rem" }}
                  >
                    Show History
                  </Button>
                </div>
                <PersonalDetails isCanNavigate={false} />
              </div>

              <div
                style={{
                  width: "100%",
                  flex: 1,
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                }}
              >
                <ReviewCandForm
                  onSaved={() => {
                    onClose(true);
                  }}
                  onCancel={() => onClose(false)}
                />
              </div>
              <div
                style={{
                  width: "100%",
                  display: isShowHistory ? "flex" : "none",
                  justifyContent: "space-between",
                  gap: "10px",
                }}
              >
                <div style={{ width: "100%" }}>
                  <div
                    style={{
                      padding: "1.5rem 0 0.2rem 0",
                      color: "#149bed",
                      fontSize: "1rem",
                      fontWeight: 500,
                    }}
                  >
                    Duplicates cv`s
                  </div>
                  <CandDupCvsList
                    candPosCvId={candsStore.candDisplay?.posCvId}
                  />
                </div>
                <div style={{ width: "100%" }}>
                  {candsStore.candDisplay &&
                    candsStore.candDisplay.posStages &&
                    candsStore.candDisplay.posStages?.length > 0 && (
                      <>
                        <div
                          style={{
                            padding: "1.5rem 0 0.2rem 0",
                            color: "#149bed",
                            fontSize: "1rem",
                            fontWeight: 500,
                          }}
                        >
                          History
                        </div>
                        <div
                          style={{
                            border: "1px solid #ffdcdc",
                            padding: "0.5rem 1rem 1rem 1rem",
                          }}
                        >
                          <CandsPosStagesList
                            cand={candsStore.candDisplay}
                            candsSource={CandsSourceEnum.AllCands}
                            isCanNavigate={false}
                          />
                        </div>
                      </>
                    )}
                </div>
              </div>
            </div>
          </Grid>
        </Grid>
      </DialogContent>
    </Dialog>
  );
});
