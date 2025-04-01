import { observer } from "mobx-react";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../../Hooks/useStore";
import { Grid, Link } from "@mui/material";
import { CandsSourceEnum, EmailTypeEnum } from "../../models/GeneralEnums";
import { useCallback, useEffect, useRef, useState } from "react";
import { isMobile } from "react-device-detect";

interface IProps {
  isCanNavigate?: boolean;
}

export const PersonalDetails = observer(({ isCanNavigate = true }: IProps) => {
  const { candsStore, generalStore, positionsStore } = useStore();
  const [candidateName, setCandidateName] = useState("");

  useEffect(() => {
    getCandName();
  }, [candsStore.candDisplay]);

  useEffect(() => {
    if (candsStore.candDisplay) {
      const posCand = candsStore.posCandsList.find(
        (x) => x.candidateId === candsStore.candDisplay?.candidateId
      );

      if (posCand) {
        candsStore.displayCv(posCand, CandsSourceEnum.Position);
      }
    }
  }, [candsStore.posCandsList]);

  const getCandName = useCallback(() => {
    let fullName = `${candsStore.candDisplay?.firstName || ""} ${
      candsStore.candDisplay?.lastName || ""
    }`;

    if (!fullName.trim()) {
      fullName = "Name not found";
    }

    setCandidateName(fullName);
  }, []);

  const handlePositionClick = useCallback(
    async (event: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => {
      if (isCanNavigate) {
        await positionsStore.positionClick(
          positionsStore.candDisplayPosition!.id,
          true
        );
        candsStore.setDisplayCandOntopPCList();
      }
    },
    []
  );

  return (
    <>
      <Grid
        container
        sx={{
          direction: "rtl",
        }}
      >
        <Grid item xs={12}>
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
                <Link href="#" onClick={handlePositionClick}>
                  {positionsStore.candDisplayPosition?.name || ""}
                  &nbsp;-&nbsp;
                  {positionsStore.candDisplayPosition?.customerName || ""}
                </Link>
              )}
            </Grid>
            <Grid
              item
              xs={12}
              sx={{
                paddingTop: "1rem",
              }}
            >
              <Grid
                container
                sx={{
                  gap: "1rem",
                }}
              >
                <Grid item>
                  <Link
                    sx={{ whiteSpace: "nowrap" }}
                    href="#"
                    onClick={() => {
                      generalStore.showCandFormDialog = true;
                    }}
                  >
                    {candidateName}{" "}
                    {candsStore.candDisplay?.city && (
                      <span style={{ color: "gray" }}>
                        {" - "} {candsStore.candDisplay?.city}
                      </span>
                    )}
                  </Link>
                </Grid>
                <Grid item>
                  <Link
                    href="#"
                    onClick={() => {
                      generalStore.showEmailDialog = EmailTypeEnum.Candidate;
                    }}
                  >
                    {candsStore.candDisplay?.email}{" "}
                  </Link>
                  &nbsp;
                </Grid>
                <Grid item>
                  <a href={"tel:" + candsStore.candDisplay?.phone}>
                    {candsStore.candDisplay?.phone}
                  </a>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </>
  );
});
