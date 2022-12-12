import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../Hooks/useStore";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { Grid } from "@mui/material";

export const Cv = observer(() => {
  const { cvsStore } = useStore();

  return (
    <Grid container>
      <Grid item xs={12} md={8}>
        <PdfViewer />
      </Grid>
      <Grid item xs={12} md={4}>
        <CvsListWrapper />
      </Grid>
    </Grid>
  );
});
