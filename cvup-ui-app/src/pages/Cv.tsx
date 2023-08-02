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
  const scrollRef = useRef<any>(null);

  useEffect(() => {
    scrollRef.current.scrollTop = 0;
  }, [candsStore.candDisplay]);

  useEffect(() => {
    if (candsStore.candDisplay?.posStages) {
      setPosStage(
        candsStore.candDisplay?.posStages.find(
          (x) => x.id === positionsStore.candDisplayPosition?.id
        )
      );
    }
  }, [candsStore.candDisplay, positionsStore.candDisplayPosition]);

  const candidateName = () => {
    let fullName = `${candsStore.candDisplay?.firstName || ""} ${
      candsStore.candDisplay?.lastName || ""
    }`;

    if (!fullName.trim()) {
      fullName = "Name not found";
    }

    return fullName;
  };

  return (
    <div ref={scrollRef} className={styles.scrollCv}>
      <Grid
        container
        sx={{
          paddingTop: "1rem",
          direction: "rtl",
          padding: "1rem 1.5rem 0 1rem",
        }}
      >
        <Grid item xs={12} lg={12}>
          <Grid
            container
            sx={{
              display: "flex",
              gap: "0.71rem",
              color: "#7b84ff",
              alignItems: "center",
              fontWeight: 700,
              // justifyContent: "center",
              fontSize: "1.1rem",
            }}
          >
            <Grid item xs="auto" lg="auto">
              {positionsStore.candDisplayPosition && (
                <Link
                  href="#"
                  onClick={() => {
                    positionsStore.candDisplayPositionClick(
                      positionsStore.candDisplayPosition!.id
                    );
                  }}
                >
                  {positionsStore.candDisplayPosition?.name || ""}
                  &nbsp;&nbsp;&nbsp;&nbsp;
                  {positionsStore.candDisplayPosition?.customerName || ""}
                </Link>
              )}
            </Grid>
          </Grid>

          <Grid
            container
            pt={1}
            sx={{
              display: "flex",
              gap: "0.71rem",
              color: "#0090d7",
              alignItems: "center",
            }}
          >
            <Grid item xs="auto" lg="auto">
              <Link
                href="#"
                onClick={() => {
                  generalStore.showCandFormDialog = true;
                }}
              >
                {candidateName()}
              </Link>
            </Grid>
            <Grid item xs="auto" lg="auto">
              <Link
                href="#"
                onClick={() => {
                  generalStore.showEmailDialog = EmailTypeEnum.Candidate;
                }}
              >
                {candsStore.candDisplay?.email}{" "}
              </Link>
            </Grid>
            <Grid item xs="auto" lg="auto">
              <a href={"tel:" + candsStore.candDisplay?.phone}>
                {candsStore.candDisplay?.phone}
              </a>
            </Grid>
            {posStage?.t && (
              <Grid
                item
                xs="auto"
                lg="auto"
                sx={{ display: "flex", alignItems: "center" }}
              >
                <FormControl variant="standard" sx={{ m: 1, minWidth: 120 }}>
                  <Select
                    sx={{
                      direction: "ltr",
                      "& .MuiSelect-select": {
                        color: candsStore.posStages?.find(
                          (x) => x.stageType === posStage?.t
                        )?.color,
                        fontWeight: "bold",
                      },
                    }}
                    value={posStage?.t}
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
                <span style={{ direction: "ltr" }}>Status:</span>
              </Grid>
            )}
          </Grid>
          <div
            style={{
              display: "flex",
              flexDirection: "row-reverse",
              fontSize: "0.7rem",
              fontWeight: 700,
            }}
          >
            {candsStore.candDisplay?.reviewDate &&
              format(
                new Date(candsStore.candDisplay?.reviewDate),
                "MMM d, yyyy"
              )}{" "}
          </div>
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

      <PdfViewer />
    </div>
  );
});
