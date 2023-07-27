import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";
import { Grid, Link } from "@mui/material";
import { EmailTypeEnum } from "../models/GeneralEnums";

export const Cv = observer(() => {
  const { candsStore, authStore, generalStore, positionsStore } = useStore();

  return (
    <div className={styles.scrollCv}>
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
            }}
          >
            <Grid item xs="auto" lg="auto">
              {candsStore.candDisplay?.position && (
                <Link
                  href="#"
                  onClick={() => {
                    positionsStore.positionClick(
                      candsStore.candDisplay!.position!.id,
                      candsStore.candDisplay
                    );
                  }}
                >
                  {candsStore.candDisplay?.position?.name || ""}
                  &nbsp;&nbsp;&nbsp;&nbsp;
                  {candsStore.candDisplay?.position?.customerName || ""}
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
                {(candsStore.candDisplay?.firstName || "") +
                  " " +
                  (candsStore.candDisplay?.lastName || "")}
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
              <a href="tel:+4733378901">{candsStore.candDisplay?.phone}</a>
            </Grid>
          </Grid>
          <div className="qlCustom">
            <pre
              style={{
                whiteSpace: "break-spaces",
                direction: authStore.isRtl ? "rtl" : "ltr",
                textAlign: authStore.isRtl ? "right" : "left",
                fontFamily: "inherit",
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
