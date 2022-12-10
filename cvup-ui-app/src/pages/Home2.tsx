import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../Hooks/useStore";
import { Worker } from "@react-pdf-viewer/core";
import { Viewer } from "@react-pdf-viewer/core";
import { defaultLayoutPlugin } from "@react-pdf-viewer/default-layout";
import "@react-pdf-viewer/core/lib/styles/index.css";
import "@react-pdf-viewer/default-layout/lib/styles/index.css";
import { CvsListWrapper } from "../components/cvs/CvsListWrapper";

export const Home2: React.FC = observer(() => {
  const { cvsStore } = useStore();
  const defaultLayoutPluginInstance = defaultLayoutPlugin();

  return (
    <div style={{ display: "flex", width: "100%" }}>
      <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
        <div
          style={{
            border: "1px solid rgba(0, 0, 0, 0.3)",
            height: "99vh",
            width: "100%",
          }}
        >
          <Viewer
            fileUrl={`https://localhost:7217/api/DD?id=${cvsStore.cvId}`}
            plugins={[defaultLayoutPluginInstance]}
          />
        </div>
      </Worker>
      <div>
        <CvsListWrapper />
      </div>
    </div>
  );
});
