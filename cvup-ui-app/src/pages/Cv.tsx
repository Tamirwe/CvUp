import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";
import { FormControl, Grid, Link, MenuItem, Select } from "@mui/material";
import { EmailTypeEnum } from "../models/GeneralEnums";
import { useEffect, useRef, useState } from "react";
import { ICandPosStage } from "../models/GeneralModels";
import { format } from "date-fns";

export const Cv = observer(() => {
  const { candsStore, authStore, generalStore, positionsStore } = useStore();
  const [posStage, setPosStage] = useState<ICandPosStage | undefined>();
  const [candidateName, setCandidateName] = useState("");
  const scrollRef = useRef<any>(null);

  useEffect(() => {
    scrollRef.current.scrollTop = 0;

    if (candsStore.candDisplay) {
      getCandName();
    }
  }, [candsStore.candDisplay]);

  useEffect(() => {
    if (candsStore.candDisplay?.posStages) {
      setPosStage(
        candsStore.candDisplay?.posStages.find(
          (x) => x._pid === positionsStore.candDisplayPosition?.id
        )
      );
    }
  }, [candsStore.candDisplay, positionsStore.candDisplayPosition]);

  const getCandName = () => {
    let fullName = `${candsStore.candDisplay?.firstName || ""} ${
      candsStore.candDisplay?.lastName || ""
    }`;

    if (!fullName.trim()) {
      fullName = "Name not found";
    }

    setCandidateName(fullName);
  };

  return (
    <div ref={scrollRef} className={styles.scrollCv}>
      {candsStore.candDisplay && (
        <Grid
          container
          sx={{
            paddingTop: "1rem",
            direction: "rtl",
            padding: "1rem 1.5rem 1rem 1rem",
          }}
        >
          <Grid item xs={12} lg={6}>
            <Grid container>
              <Grid
                item
                xs={12}
                sx={{
                  color: "#7b84ff",
                  fontWeight: 700,
                  fontSize: "1.1rem",
                }}
              >
                {positionsStore.candDisplayPosition && (
                  <Link
                    href="#"
                    onClick={async () => {
                      await positionsStore.positionClick(
                        positionsStore.candDisplayPosition!.id,
                        true
                      );
                      candsStore.setDisplayCandOntopPCList();
                    }}
                  >
                    {positionsStore.candDisplayPosition?.name || ""}
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    {positionsStore.candDisplayPosition?.customerName || ""}
                  </Link>
                )}
              </Grid>
              <Grid
                item
                xs={12}
                sx={{ display: "flex", gap: "1rem", paddingTop: "0.5rem" }}
              >
                <Link
                  href="#"
                  onClick={() => {
                    generalStore.showCandFormDialog = true;
                  }}
                >
                  {candidateName}
                </Link>
                <Link
                  href="#"
                  onClick={() => {
                    generalStore.showEmailDialog = EmailTypeEnum.Candidate;
                  }}
                >
                  {candsStore.candDisplay?.email}{" "}
                </Link>
                <a href={"tel:" + candsStore.candDisplay?.phone}>
                  {candsStore.candDisplay?.phone}
                </a>
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={12} lg={6}>
            <Grid container>
              {posStage?._dt && (
                <Grid
                  item
                  xs={12}
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    textAlign: "left",
                    flexDirection: "row-reverse",
                    gap: 1,
                  }}
                >
                  <span style={{ direction: "ltr" }}>Status:</span>
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
                        await candsStore.updateCandPositionStatus(
                          e.target.value
                        );
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
                  <span>
                    {" "}
                    {format(new Date(posStage?._dt), "MMM d, yyyy")}{" "}
                  </span>
                </Grid>
              )}
              {posStage?._ec && (
                <Grid
                  item
                  xs={12}
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    textAlign: "left",
                    flexDirection: "row-reverse",
                    gap: 1,
                    paddingTop: "1rem",
                  }}
                >
                  <span style={{ direction: "ltr" }}>Sent to customer:</span>
                  <span>
                    {" "}
                    {posStage?._ec &&
                      format(new Date(posStage?._ec), "MMM d, yyyy")}{" "}
                  </span>
                </Grid>
              )}
            </Grid>
          </Grid>
          <Grid item xs={12} lg={12}>
            {candsStore.candDisplay?.reviewDate && (
              <div
                title="Review updated date"
                style={{
                  display: "flex",
                  flexDirection: "row-reverse",
                  fontSize: "0.7rem",
                  fontWeight: 700,
                  paddingTop: "1rem",
                  gap: 2,
                }}
              >
                <span style={{ direction: "ltr" }}>Review:</span>
                {format(
                  new Date(candsStore.candDisplay?.reviewDate),
                  "MMM d, yyyy"
                )}{" "}
              </div>
            )}
            <div className="qlCustom">
              <pre
                style={{
                  whiteSpace: "break-spaces",
                  direction: authStore.isRtl ? "rtl" : "ltr",
                  textAlign: authStore.isRtl ? "right" : "left",
                  fontFamily: "inherit",
                  margin: 0,
                }}
                dangerouslySetInnerHTML={{
                  __html: candsStore.candDisplay?.review || "",
                }}
              ></pre>
            </div>
          </Grid>
        </Grid>
      )}
      {candsStore.candDisplay && <PdfViewer />}
    </div>
  );
});
