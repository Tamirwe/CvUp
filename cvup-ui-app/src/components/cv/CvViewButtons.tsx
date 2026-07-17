import { observer } from "mobx-react";
import { Button, Grid, Stack } from "@mui/material";
import { useState } from "react";
import { isMobile } from "react-device-detect";
import { MdKeyboardArrowDown, MdKeyboardArrowUp } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  EmailTypeEnum,
} from "../../models/GeneralEnums";

interface IProps {
  setReview: (review: string) => void;
}

export const CvViewButtons = observer(({ setReview }: IProps) => {
  const { candsStore, generalStore } = useStore();
  const [showActionButtons, setShowActionButtons] = useState(false);

  return (
    <Grid item xs={12}>
      <Stack
        direction="row"
        flexWrap="wrap"
        gap={1}
        alignItems="center"
        sx={{ paddingTop: "0.5rem" }}
      >
        <Button
          size="small"
          variant="outlined"
          onClick={() => {
            if (isMobile) {
              generalStore.showReviewCandDialog =
                !generalStore.showReviewCandDialog;
            } else {
              setReview(candsStore.candDisplay?.review || "");
              generalStore.showInterviewFullDialog =
                !generalStore.showInterviewFullDialog;
            }
          }}
        >
          Review
        </Button>

        <Button
          size="small"
          variant="outlined"
          color="success"
          onClick={() =>
            (generalStore.showEmailDialog = EmailTypeEnum.Contact)
          }
        >
          Customer Email
        </Button>
        <Button
          size="small"
          variant="outlined"
          onClick={() => {
            generalStore.showCandFormDialog = true;
          }}
        >
          Edit cand
        </Button>
        <Button
          size="small"
          variant="outlined"
          onClick={() => {
            generalStore.showCustomerReviewCandDialog =
              !generalStore.showCustomerReviewCandDialog;
          }}
        >
          Customer review
        </Button>
        <Button
          size="small"
          variant="outlined"
          color="secondary"
          onClick={() => {
            generalStore.showEmailDialog = EmailTypeEnum.Candidate;
          }}
        >
          Cand. Email
        </Button>
        <Button
          size="small"
          variant="outlined"
          title={showActionButtons ? "Hide buttons" : "Show buttons"}
          onClick={() => {
            setShowActionButtons(!showActionButtons);
          }}
          sx={{ minWidth: "2rem", fontSize: "1.4rem" }}
        >
          {showActionButtons ? <MdKeyboardArrowUp /> : <MdKeyboardArrowDown />}
        </Button>
      </Stack>
      {showActionButtons && (
        <Stack
          direction="row"
          flexWrap="wrap"
          gap={1}
          sx={{ paddingTop: "0.5rem" }}
        >
          {candsStore.candDisplay?.isBlackList ? (
            <Button
              size="small"
              variant="outlined"
              onClick={async () => {
                const candidateId = candsStore.candDisplay?.candidateId;

                if (!candidateId) {
                  return;
                }

                const res = await candsStore.removeBlackCand(candidateId);

                if (res.isSuccess) {
                  await generalStore.alertConfirmDialog(
                    AlertConfirmDialogEnum.Alert,
                    "Remove from Black List",
                    "Candidate removed from black list successfully",
                  );
                  window.location.reload();
                } else {
                  generalStore.alertSnackbar(
                    "error",
                    "Failed to remove from black list",
                  );
                }
              }}
            >
              Remove from Black List
            </Button>
          ) : (
            <Button
              size="small"
              variant="outlined"
              onClick={async () => {
                const candidateId = candsStore.candDisplay?.candidateId;

                if (!candidateId) {
                  return;
                }

                const res = await candsStore.addBlackCand(candidateId);

                if (res.isSuccess) {
                  await generalStore.alertConfirmDialog(
                    AlertConfirmDialogEnum.Alert,
                    "Add to Black List",
                    "Candidate added to black list successfully",
                  );
                  window.location.reload();
                } else {
                  generalStore.alertSnackbar(
                    "error",
                    "Failed to add to black list",
                  );
                }
              }}
            >
              Add to Black List
            </Button>
          )}
          <Button
            size="small"
            variant="outlined"
            onClick={() => {
              generalStore.showRestoreReviewDialog =
                !generalStore.showRestoreReviewDialog;
            }}
          >
            Restore Interview
          </Button>
          <Button
            size="small"
            variant="outlined"
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
            Download original file
          </Button>
        </Stack>
      )}
      {showActionButtons && (
        <Stack
          direction="row"
          flexWrap="wrap"
          gap={1}
          sx={{ paddingTop: "0.5rem" }}
        >
          <Button
            size="small"
            variant="outlined"
            disabled={!candsStore.candDisplay?.email}
            onClick={async () => {
              const email = candsStore.candDisplay?.email;

              if (!email) {
                return;
              }

              const isMerge = await generalStore.alertConfirmDialog(
                AlertConfirmDialogEnum.Confirm,
                "Merge Candidates",
                `Are you sure you want to merge all duplicate candidates with email "${email}"?`,
              );

              if (!isMerge) {
                return;
              }

              const res = await candsStore.mergeDuplicateCandsByEmail(email);

              if (res.isSuccess) {
                candsStore.candDisplay = undefined;
                window.location.reload();
              } else {
                generalStore.alertSnackbar(
                  "error",
                  "Failed to merge candidates",
                );
              }
            }}
          >
            Merge Candidates
          </Button>
          <Button
            size="small"
            variant="outlined"
            onClick={async () => {
              const isDelete = await generalStore.alertConfirmDialog(
                AlertConfirmDialogEnum.Confirm,
                "Delete Cv",
                "Are you sure you want to delete this cv? The candidate will also be deleted if this is their last cv.",
              );

              if (isDelete) {
                const res = await candsStore.deleteCv();

                if (res.isSuccess) {
                  await generalStore.alertConfirmDialog(
                    AlertConfirmDialogEnum.Alert,
                    "Delete Cv",
                    "Cv deleted successfully",
                  );
                  window.location.reload();
                } else {
                  generalStore.alertSnackbar("error", "Failed to delete cv");
                }
              }
            }}
          >
            Delete CV
          </Button>
          <Button
            size="small"
            variant="outlined"
            onClick={async () => {
              const isDelete = await generalStore.alertConfirmDialog(
                AlertConfirmDialogEnum.Confirm,
                "Delete Candidate",
                "Are you sure you want to delete this candidate? All of their cv's will also be deleted.",
              );

              if (isDelete) {
                const res = await candsStore.deleteCandidate();

                if (res.isSuccess) {
                  await generalStore.alertConfirmDialog(
                    AlertConfirmDialogEnum.Alert,
                    "Delete Candidate",
                    "Candidate deleted successfully",
                  );
                  window.location.reload();
                } else {
                  generalStore.alertSnackbar(
                    "error",
                    "Failed to delete candidate",
                  );
                }
              }
            }}
          >
            Delete Candidate
          </Button>
        </Stack>
      )}
    </Grid>
  );
});
