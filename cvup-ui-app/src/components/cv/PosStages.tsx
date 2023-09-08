import { observer } from "mobx-react";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../../Hooks/useStore";
import styles from "./CvView.module.scss";
import { FormControl, Grid, MenuItem, Select } from "@mui/material";
import { useEffect, useState } from "react";
import { ICandPosHistory, ICandPosStage } from "../../models/GeneralModels";
import { format } from "date-fns";
import { isMobile } from "react-device-detect";
import { StageDateDialog } from "./StageDateDialog";

export const PosStages = observer(() => {
  const { candsStore, positionsStore } = useStore();

  const [posStage, setPosStage] = useState<ICandPosStage | undefined>();
  const [showStageDateDialog, setShowStageDateDialog] = useState(false);
  const [stageTypeToEdit, setStageTypeToEdit] = useState("");
  const [posHistory, setPosHistory] =
    useState<
      { name: string; type: string; stageDate?: Date; color?: string }[]
    >();

  useEffect(() => {
    const candDisplay = candsStore.candDisplay;

    if (candDisplay?.posStages) {
      setPosStage(
        candDisplay?.posStages.find(
          (x) => x._pid === positionsStore.candDisplayPosition?.id
        )
      );

      const history = [];
      let color;

      if (candDisplay.candPosHistory?.callEmailToCandidate) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "sent_candidate_call_request"
        )?.color;
        history.push({
          name: "Call request",
          type: "sent_candidate_call_request",
          stageDate: candDisplay.candPosHistory?.callEmailToCandidate,
          color,
        });
      }
      if (candDisplay.candPosHistory?.emailToContact) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "cv_sent_to_customer"
        )?.color;
        history.push({
          name: "Sent to customer",
          type: "cv_sent_to_customer",
          stageDate: candDisplay.candPosHistory?.emailToContact,
          color,
        });
      }
      if (candDisplay.candPosHistory?.customerInterview) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "customer_interview"
        )?.color;
        history.push({
          name: "Customer interview",
          type: "customer_interview",
          stageDate: candDisplay.candPosHistory?.customerInterview,
          color,
        });
      }
      if (candDisplay.candPosHistory?.removeCandidacy) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "withdraw_candidacy"
        )?.color;
        history.push({
          name: "Withdraw candidacy",
          type: "withdraw_candidacy",
          stageDate: candDisplay.candPosHistory?.removeCandidacy,
          color,
        });
      }
      if (candDisplay.candPosHistory?.rejected) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "rejected"
        )?.color;
        history.push({
          name: "Rejected",
          type: "rejected",
          stageDate: candDisplay.candPosHistory?.rejected,
          color,
        });
      }
      if (candDisplay.candPosHistory?.accepted) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "accepted"
        )?.color;
        history.push({
          name: "Accepted",
          type: "accepted",
          stageDate: candDisplay.candPosHistory?.accepted,
          color,
        });
      }
      if (candDisplay.candPosHistory?.rejectEmailToCandidate) {
        color = candsStore.posStages?.find(
          (x) => x.stageType === "rejected_email_sent"
        )?.color;
        history.push({
          name: "Rejected email sent",
          type: "rejected_email_sent",
          stageDate: candDisplay.candPosHistory?.rejectEmailToCandidate,
          color,
        });
      }

      setPosHistory(history);
    }
  }, [candsStore.candDisplay, positionsStore.candDisplayPosition]);

  const handlePosStageClick = (stageType: string) => {
    setStageTypeToEdit(stageType);
    setShowStageDateDialog(true);
  };

  const handleRemoveStage = async () => {
    setShowStageDateDialog(false);
    await candsStore.removePosStage(stageTypeToEdit);
  };

  const handleUpdateStageDate = async (newDate: Date) => {
    setShowStageDateDialog(false);
    await candsStore.updatePosStageDate(stageTypeToEdit, newDate);
  };

  return (
    <Grid container>
      {showStageDateDialog && (
        <StageDateDialog
          isOpen={showStageDateDialog}
          onClose={() => setShowStageDateDialog(false)}
          onRemoveStage={handleRemoveStage}
          onNewStageDate={handleUpdateStageDate}
        />
      )}
      {posStage?._dt && (
        <Grid
          item
          xs={12}
          sx={{
            display: "flex",
            alignItems: "center",
            textAlign: "left",
            flexDirection: "row-reverse",
            gap: 1.5,
            paddingTop: isMobile ? 1.5 : 0,
            paddingBottom: "1.2rem",
          }}
        >
          <span style={{ direction: "ltr" }}>Stage:</span>
          <FormControl variant="standard" sx={{ minWidth: 120 }}>
            <Select
              sx={{
                direction: "ltr",
                "& .MuiSelect-select": {
                  color: candsStore.posStages?.find(
                    (x) => x.stageType === posStage?._tp
                  )?.color,
                  fontWeight: "bold",
                },
              }}
              value={posStage?._tp}
              onChange={async (e) => {
                await candsStore.updateCandPositionStatus(e.target.value);
              }}
            >
              {candsStore.posStages?.map((item, ind) => {
                // console.log(key, index);
                return (
                  <MenuItem
                    sx={{ color: item.color }}
                    key={ind}
                    value={item.stageType}
                  >
                    {item.name}
                  </MenuItem>
                );
              })}
            </Select>
          </FormControl>
          <span style={{ whiteSpace: "nowrap", fontSize: "0.75rem" }}>
            {" "}
            {format(new Date(posStage?._dt), "MMM d, yyyy")}{" "}
          </span>
        </Grid>
      )}

      {posHistory?.length ? (
        <Grid
          item
          xs={12}
          sx={{
            textAlign: "left",
            textDecoration: "underline",
            direction: "ltr",
          }}
        >
          Stages history:
        </Grid>
      ) : (
        ""
      )}
      {posHistory?.map((item, ind) => {
        return (
          <Grid
            key={ind}
            item
            xs={12}
            sx={{
              display: "flex",
              flexDirection: "row-reverse",
              gap: 1,
              paddingTop: "0.4rem",
              color: item.color,
              cursor: "pointer",
              fontWeight: "bold",
            }}
            onClick={() => handlePosStageClick(item.type)}
          >
            <span style={{ direction: "ltr" }}>{item.name}:</span>
            <span style={{ fontSize: "0.7rem" }}>
              {item.stageDate &&
                format(new Date(item.stageDate), "MMM d, yyyy")}{" "}
            </span>
          </Grid>
        );
      })}
    </Grid>
  );
});
