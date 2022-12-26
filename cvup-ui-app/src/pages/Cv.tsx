import { observer } from "mobx-react";
import { PdfViewer } from "../components/pdfViewer/PdfViewer";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";
import { Grid } from "@mui/material";
import { PositionsListWrapper } from "../components/positions/PositionsListWrapper";

export const Cv = observer(() => {
  return (
    <div className="cv-layout">
      <div className="columns">
        <div className="column">
          <PositionsListWrapper />
        </div>
        <div className="column">
          <PdfViewer />
        </div>
        <div className="column">
          <CvsListWrapper />
        </div>
      </div>
    </div>
  );
});
