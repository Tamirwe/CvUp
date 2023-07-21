import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import "react-quill/dist/quill.snow.css";
import { useStore } from "../Hooks/useStore";
import styles from "./Cv.module.scss";
import { Grid, IconButton } from "@mui/material";
import { CiEdit } from "react-icons/ci";

export const Cv = observer(() => {
  const { candsStore, authStore, generalStore } = useStore();

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
              color: "#0090d7",
              alignItems: "center",
            }}
          >
            <Grid item xs="auto" lg="auto">
              <IconButton
                color="primary"
                onClick={() => {
                  generalStore.showCandFormDialog = true;
                }}
              >
                <CiEdit />
              </IconButton>
            </Grid>
            <Grid item xs="auto" lg="auto">
              {(candsStore.candDisplay?.firstName || "") +
                " " +
                (candsStore.candDisplay?.lastName || "")}
            </Grid>
            <Grid item xs="auto" lg="auto">
              <div>{candsStore.candDisplay?.email}</div>
            </Grid>
            <Grid item xs="auto" lg="auto">
              <div>{candsStore.candDisplay?.phone}</div>
            </Grid>
          </Grid>
          <div className="qlCustom">
            <pre
              style={{
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
