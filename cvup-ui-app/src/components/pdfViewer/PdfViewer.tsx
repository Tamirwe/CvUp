import { observer } from "mobx-react";
import { Viewer } from "@react-pdf-viewer/core";
import { defaultLayoutPlugin } from "@react-pdf-viewer/default-layout";
import "@react-pdf-viewer/core/lib/styles/index.css";
import "@react-pdf-viewer/default-layout/lib/styles/index.css";
import { useStore } from "../../Hooks/useStore";

export const PdfViewer = observer(() => {
  const { cvsStore } = useStore();
  const defaultLayoutPluginInstance = defaultLayoutPlugin();

  return (
    <>
      {cvsStore.cvId && (
        <Viewer
          fileUrl={`https://localhost:7217/api/DD?id=${cvsStore.cvId}`}
          //   plugins={[defaultLayoutPluginInstance]}
        />
      )}
    </>
  );
});
